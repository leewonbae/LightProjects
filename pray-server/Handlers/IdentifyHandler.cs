using pray_server.Attributes;
using Snowpipe.Commons.Packets;

namespace pray_server.Handlers
{
    [PacketHandler]
    public class IdentifyHandler : IHandler
    {
        public IPacket Execute(string packetBody)
        {
            return new ResIdentify();
        }
    }
}
