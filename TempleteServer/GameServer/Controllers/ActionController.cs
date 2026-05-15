using GameServer.Exceptions;
using GameServer.Handlers;
using GameServer.Helpers;
using GameServer.Managers;
using GameServer.Redis;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Snowpipe.Commons;
using Snowpipe.Commons.Packets;
using System.Threading.Tasks;

namespace GameServer.Controllers
{
    [ApiController]
    [Route("action")]
    public class ActionController : ControllerBase
    {
        private const string REQ_HEADER_SESSION_TOKEN = "Session-Token";
        private const string RES_HEADER_SERVER_DATETIME = "Server-DateTime";
        private const string REQ_HEADER_REQUEST_TOKEN = "Request-Token";

        private readonly RedisRepository _redisRepository;
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger _logger;
        public ActionController(ILogger<ActionController> logger, IServiceProvider serviceProvider,
               RedisRepository redisManager)
        {
            _logger = logger;

            _serviceProvider = serviceProvider;

            _redisRepository = redisManager;
        }

        // 진입점 이기 때문에, ASYNC/AWAIT 패턴을 사용하여 비동기적으로 처리하는 것이 좋습니다.
        // Microsoft 권장 사항: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-8.0#asynchronous-io
        [HttpPost]
        public async Task<BaseResPacket> Index(string packetName, [FromBody] BaseReqPacket packet, bool isJsonTest = false)
        {
            Response.Headers.Append(RES_HEADER_SERVER_DATETIME, ServerDateTime.Now.ToUniversalTime().ToString("u"));

            AccountInfoCache accountInfoCache = null;
            var cleanPacketName = packetName!.StartsWith("Req") ? packetName.Substring(3).ToLower() : packetName.ToLower();

            BaseResPacket baseResPacket = new BaseResPacket();
            try
            {
                var handlerWrapperType = ServiceCollectionRegister.GetHandlerWrapper(cleanPacketName);
                var handlerWrapper = _serviceProvider.GetRequiredService(handlerWrapperType) as IHandlerWrapper;

                // session token이 필요한 패킷인데, session token이 없는 경우 예외 처리
                if (handlerWrapper.NeedToLogin)
                {
                    if (!Request.Headers.TryGetValue(REQ_HEADER_SESSION_TOKEN, out var sessionToken))
                    {
                        throw new GameServerException(E_PACKET_ERROR_CODE.NEED_TO_LOGIN, $"Packet [{packetName}] requires login.");
                    }

                    accountInfoCache = await _redisRepository.GetAccountInfoCacheAsync(sessionToken);
                    VerifySessionToken(accountInfoCache, packetName);
                }

                _logger.LogInformation($"[Request] [PacketName] : [{packetName}] [AccountId] : [{accountInfoCache?.AccountId}] [Body] : [{packet.PacketBody}]");

                var res = await handlerWrapper.ExecuteAsync(accountInfoCache, packet.PacketBody);

                baseResPacket.PacketBody = JsonConvert.SerializeObject(res);
            }
            catch (GameServerException gsex)
            {
                _logger.LogError($"[GameServerException] : {gsex.Message}");
                baseResPacket.ErrorCode = gsex.ErrorCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Exception] : {ex.Message}");
                baseResPacket.ErrorCode = E_PACKET_ERROR_CODE.SERVER_ERROR;
            }

            return baseResPacket;
        }

        private void VerifySessionToken(AccountInfoCache accountInfoCache, string packetName)
        {
            switch (accountInfoCache.LoginStatusType)
            {
                case E_LOGIN_STATUS_TYPE.DUPLICATED: // 중복된 상태라면
                    throw new GameServerException(E_PACKET_ERROR_CODE.DUPLICATED_LOGIN, $"Duplicated login :{accountInfoCache.AccountId} : {accountInfoCache.SessionToken}");

                case E_LOGIN_STATUS_TYPE.KICKED: // 킥된 유저
                    throw new GameServerException(E_PACKET_ERROR_CODE.KICKED_TOKEN, $"Kicked user : {accountInfoCache.AccountId} : {accountInfoCache.SessionToken}");

                case E_LOGIN_STATUS_TYPE.BANNED: // 밴 처리된 유저
                    throw new GameServerException(E_PACKET_ERROR_CODE.BANNED_TOKEN, $"Banned user : {accountInfoCache.AccountId} : {accountInfoCache.SessionToken}");

                default:
                    throw new GameServerException(E_PACKET_ERROR_CODE.NEED_TO_LOGIN, $"Packet [{packetName}] requires login.");
            }
        }

        [HttpPost]
        [Route("DoJson")]
        public Task<BaseResPacket> DoJsonAsync(string packetName, string jsonBody)
        {
            var baseReqPacket = new BaseReqPacket { PacketBody = jsonBody };

            return Index(packetName, baseReqPacket, true);

        }
    }
}
