using GameServer.Redis.Models;
using Snowpipe.Commons.Packets;

namespace GameServer.Handlers
{
    public interface IHandler<TReq, TRes>
        where TReq : IPacket
        where TRes : IPacket
    {
        bool NeedToLogin => true;
        Task<TRes> Execute(AccountInfoCache accountInfoCache, TReq packet);
    }
}
