using pray_server.Attributes;
using Snowpipe.Commons.Packets;

namespace pray_server.Handlers
{
    [PacketHandler]
    public class LoginHandler : IHandler
    {
        public IPacket Execute(string jsonBody)
        {
            return new ResLogin();
        }
    }
}
