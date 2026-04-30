using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using pray_server.Commons;
using pray_server.Exceptions;
using pray_server.Handlers;
using pray_server.Helpers;
using pray_server.Managers;
using pray_server.Redis.Models;
using Snowpipe.Commons.Packets;
using System.Threading.Tasks;

namespace pray_server.Controllers
{
    [ApiController]
    [Route("action")]
    public class ActionController : ControllerBase
    {
        private const string REQ_HEADER_SESSION_TOKEN = "Session-Token";
        private const string RES_HEADER_SERVER_DATETIME = "Server-DateTime";
        private const string REQ_HEADER_REQUEST_TOKEN = "Request-Token";

        private readonly AccountManager _accountManager;
        private readonly IServiceProvider _serviceProvider;
        public ActionController(IServiceProvider serviceProvider, AccountManager accountManager)
        {
            _serviceProvider = serviceProvider;

            _accountManager = accountManager;
        }

        // 진입점 이기 때문에, ASYNC/AWAIT 패턴을 사용하여 비동기적으로 처리하는 것이 좋습니다.
        // Microsoft 권장 사항: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-8.0#asynchronous-io
        [HttpPost]
        public async Task<BaseResPacket> Index(string packetName, [FromBody] BaseReqPacket packet, bool isJsonTest = false)
        {
            Response.Headers.Append(RES_HEADER_SERVER_DATETIME, ServerDateTime.Now.ToUniversalTime().ToString("u"));

            AccountInfoCache accountInfoCache;
            var cleanPacketName = packetName!.StartsWith("Req") ? packetName.Substring(3).ToLower() : packetName.ToLower();

            BaseResPacket baseResPacket = new BaseResPacket();
            try
            {
                var handlerWrapperType = ServiceCollectionRegister.GetHandlerWrapper(cleanPacketName);
                var handlerWrapper = _serviceProvider.GetRequiredService(handlerWrapperType) as IHandlerWrapper;

                // session token이 필요한 패킷인데, session token이 없는 경우 예외 처리
                accountInfoCache = VerifySessionTokenAndGetCache(handlerWrapper.NeedToLogin, cleanPacketName);

                var res = await handlerWrapper.ExecuteAsync(accountInfoCache, packet.PacketBody);

                baseResPacket.PacketBody = JsonConvert.SerializeObject(res);
            }
            catch (GameServerException gsex)
            {
                baseResPacket.ErrorCode = gsex.ErrorCode;
            }
            catch (Exception ex)
            {
                baseResPacket.ErrorCode = E_PACKET_ERROR_CODE.SERVER_ERROR;
            }

            return baseResPacket;
        }

        [HttpPost]
        [Route("json")]
        public Task<BaseResPacket> DoJsonAsync(string packetName, string jsonBody)
        {
            var baseReqPacket = new BaseReqPacket { PacketBody = jsonBody };

            return Index(packetName, baseReqPacket, true);
        }

        private AccountInfoCache VerifySessionTokenAndGetCache(bool isNeedToLoginHandler, string packetName)
        {
            if (!isNeedToLoginHandler)
            {
                return null;
            }

            if (!Request.Headers.TryGetValue(REQ_HEADER_SESSION_TOKEN, out var sessionToken))
            {
                throw new GameServerException(E_PACKET_ERROR_CODE.NEED_TO_LOGIN, $"Packet [{packetName}] requires login.");
            }

            // 레디스에 저장된 정보를 확인 
            var accountInfoCache = new AccountInfoCache();

            if (accountInfoCache == null || accountInfoCache.LoginStatusType != E_LOGIN_STATUS_TYPE.LOGINED)
            {
                if (accountInfoCache == null)
                {
                    throw new GameServerException(E_PACKET_ERROR_CODE.NEED_TO_LOGIN, $"Packet [{packetName}] requires login.");
                }

                switch (accountInfoCache.LoginStatusType)
                {
                    case E_LOGIN_STATUS_TYPE.DUPLICATED: // 중복된 상태라면
                        throw new GameServerException(E_PACKET_ERROR_CODE.DUPLICATED_LOGIN, $"Duplicated login :{accountInfoCache.AccountId} : {sessionToken}");

                    case E_LOGIN_STATUS_TYPE.KICKED: // 킥된 유저
                        throw new GameServerException(E_PACKET_ERROR_CODE.KICKED_TOKEN, $"Kicked user : {accountInfoCache.AccountId} : {sessionToken}");

                    case E_LOGIN_STATUS_TYPE.BANNED: // 밴 처리된 유저
                        throw new GameServerException(E_PACKET_ERROR_CODE.BANNED_TOKEN, $"Banned user : {accountInfoCache.AccountId} : {sessionToken}");

                    default:
                        throw new GameServerException(E_PACKET_ERROR_CODE.NEED_TO_LOGIN, $"Packet [{packetName}] requires login.");
                }
            }

            return accountInfoCache;
        }
    }
}
