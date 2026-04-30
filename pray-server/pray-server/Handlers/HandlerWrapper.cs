using pray_server.Exceptions;
using pray_server.Redis.Models;
using Snowpipe.Commons.Packets;

namespace pray_server.Handlers
{
    public interface IHandlerWrapper
    {
        Task<IPacket> ExecuteAsync(AccountInfoCache accountInfoCache, string packetBody);
        bool NeedToLogin { get; }
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

        public bool NeedToLogin => _handler.NeedToLogin;

        public async Task<IPacket> ExecuteAsync(AccountInfoCache accountInfoCache, string packetBody)
        {
            var req = Newtonsoft.Json.JsonConvert.DeserializeObject<TReq>(packetBody);
            if (req == null)
            {
                throw new GameServerException(E_PACKET_ERROR_CODE.INVALID_PACKET, "Failed to deserialize packet body.");
            }

            return await _handler.Execute(accountInfoCache, req);
        }
    }
}
