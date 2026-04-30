using pray_server.Exceptions;
using Snowpipe.Commons.Packets;

namespace pray_server.Handlers
{
    public interface IHandlerWrapper
    {
        Task<IPacket> ExecuteAsync(string packetBody);
    }

    public class HandlerWrapper<TReq, TRes> : IHandlerWrapper
        where TReq : IPacket
        where TRes : IPacket
    {
        private readonly IHandler<TReq, TRes> _handler;
        public HandlerWrapper(IHandler<TReq, TRes> handler)
        {
            _handler = handler;
        }

        public async Task<IPacket> ExecuteAsync(string packetBody)
        {
            var req = Newtonsoft.Json.JsonConvert.DeserializeObject<TReq>(packetBody);
            if (req == null)
            {
                throw new GameServerException(E_PACKET_ERROR_CODE.INVALID_PACKET, "Failed to deserialize packet body.");
            }

            return await _handler.Execute(req);
        }
    }
}
