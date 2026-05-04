using GameServer.Extensions;
using GameServer.Managers;
using GameServer.Redis.Models;
using GameServer.Services;
using Newtonsoft.Json;
using Snowpipe.Commons.Packets;

namespace GameServer.Handlers
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
