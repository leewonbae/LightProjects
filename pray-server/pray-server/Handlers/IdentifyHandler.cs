using Newtonsoft.Json;
using pray_server.Extensions;
using pray_server.Managers;
using Snowpipe.Commons.Packets;

namespace pray_server.Handlers
{
    [InjectableClass(ServiceLifetime.Scoped)]
    public class IdentifyHandler : IHandler<ReqIdentify, ResIdentify>
    {
        private readonly AccountManager _accountManager;
        public IdentifyHandler(IServiceProvider serviceProvider)
        {
            _accountManager = serviceProvider.GetRequiredService<AccountManager>();
        }

        public IPacket Execute(string packetBody)
        {
            var req = JsonConvert.DeserializeObject<ReqIdentify>(packetBody);

            return new ResIdentify();
        }

        public Task<ResIdentify> Execute(ReqIdentify packet)
        {
            throw new NotImplementedException();
        }

        public Task<IPacket> ExecuteAsync(string packetBody)
        {
            throw new NotImplementedException();
        }
    }
}
