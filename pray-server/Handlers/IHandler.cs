using Snowpipe.Commons.Packets;

namespace pray_server.Handlers
{
    public interface IHandler
    {
        bool NeedToLogin => true;
        IPacket Execute(string packetBody);
    }
}
