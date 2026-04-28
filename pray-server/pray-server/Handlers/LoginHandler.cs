using pray_server.Extensions;
using Snowpipe.Commons.Packets;

namespace pray_server.Handlers
{
    [InjectableClass(ServiceLifetime.Scoped)]
    public class LoginHandler : IHandler<ReqLogin, ResLogin>
    {
        public IPacket Execute(string jsonBody)
        {
            return new ResLogin();
        }

        public Task<ResLogin> Execute(ReqLogin packet)
        {
            throw new NotImplementedException();
        }

        public Task<IPacket> ExecuteAsync(string packetBody)
        {
            throw new NotImplementedException();
        }
    }
}
