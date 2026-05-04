using GameServer.Extensions;
using GameServer.Managers;
using GameServer.Redis;
using GameServer.Services;
using Newtonsoft.Json;
using Snowpipe.Commons.Packets;

namespace GameServer.Handlers
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
