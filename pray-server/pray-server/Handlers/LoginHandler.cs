using pray_server.Extensions;
using pray_server.Redis.Models;
using pray_server.Services;
using Snowpipe.Commons.Packets;

namespace pray_server.Handlers
{
    [InjectableClass(ServiceLifetime.Scoped)]
    public class LoginHandler : IHandler<ReqLogin, ResLogin>
    {
        public bool NeedToLogin => false;
        private readonly AccountService _accountService;
        public LoginHandler(AccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<ResLogin> Execute(AccountInfoCache accountInfoCache, ReqLogin packet)
        {
            var result = await _accountService.Login(accountInfoCache, packet);

            return new ResLogin()
            {
                GameAccountVo = result.GameAccountDto.ToVo(),
            };
        }
    }
}
