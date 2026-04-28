using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using pray_server.Exceptions;
using pray_server.Handlers;
using pray_server.Helpers;
using Snowpipe.Commons.Packets;

namespace pray_server.Controllers
{
    [ApiController]
    [Route("action")]
    public class ActionController : ControllerBase
    {
        private const string REQ_HEADER_ACCOUNT_TOKEN = "Account-Token";
        private const string RES_HEADER_SERVER_DATETIME = "Server-DateTime";
        private const string REQ_HEADER_REQUEST_TOKEN = "Request-Token";

        private readonly IServiceProvider _serviceProvider;
        public ActionController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        public BaseResPacket Index(string packetName, [FromBody] BaseReqPacket packet, bool isJsonTest = false)
        {
            Response.Headers.Append(RES_HEADER_SERVER_DATETIME, ServerDateTime.Now.ToUniversalTime().ToString("u"));

            var cleanPacketName = packetName!.StartsWith("Req") ? packetName.Substring(3).ToLower() : packetName.ToLower();

            BaseResPacket baseResPacket = new BaseResPacket();
            try
            {
                var handlerWrapperType = ServiceCollectionRegister.GetHandlerWrapper(cleanPacketName);
                var handlerWrapper = _serviceProvider.GetRequiredService(handlerWrapperType) as IHandlerWrapper;

                var res = handlerWrapper.Execute(packet.PacketBody);

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
        public BaseResPacket DoJson(string packetName, string jsonBody)
        {
            var baseReqPacket = new BaseReqPacket { PacketBody = jsonBody };

            return Index(packetName, baseReqPacket, true);
        }

    }
}
