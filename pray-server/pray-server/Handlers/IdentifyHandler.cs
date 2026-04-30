using Newtonsoft.Json;
using pray_server.Extensions;
using pray_server.Managers;
using pray_server.Redis.Models;
using pray_server.Services;
using Snowpipe.Commons.Packets;

namespace pray_server.Handlers
{
    [InjectableClass(ServiceLifetime.Scoped)]
    public class IdentifyHandler : IHandler<ReqIdentify, ResIdentify>
    {
        public bool NeedToLogin => false;
        private readonly AccountService _accountService;
        public IdentifyHandler(AccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<ResIdentify> Execute(AccountInfoCache accountInfoCache, ReqIdentify packet)
        {
            var existsAccount = await _accountService.Identify(packet);

            return new ResIdentify
            {
                ExistsAccount = existsAccount,
            };
        }
    }
}
