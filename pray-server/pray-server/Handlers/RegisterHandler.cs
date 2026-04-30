using Newtonsoft.Json;
using pray_server.Extensions;
using pray_server.Managers;
using pray_server.Redis.Models;
using pray_server.Services;
using Snowpipe.Commons.Packets;

namespace pray_server.Handlers
{
    [InjectableClass(ServiceLifetime.Scoped)]
    public class RegisterHandler : IHandler<ReqRegister, ResRegister>
    {
        public bool NeedToLogin => false;
        private readonly AccountService _accountService;
        public RegisterHandler(IServiceProvider serviceProvider)
        {
            _accountService = serviceProvider.GetRequiredService<AccountService>();
        }

        public async Task<ResRegister> Execute(AccountInfoCache accountInfoCache, ReqRegister packet)
        {
            await _accountService.Register(packet);

            return new ResRegister();
        }
    }
}
