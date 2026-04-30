using Newtonsoft.Json;
using pray_server.Extensions;
using pray_server.Managers;
using pray_server.Services;
using Snowpipe.Commons.Packets;

namespace pray_server.Handlers
{
    [InjectableClass(ServiceLifetime.Scoped)]
    public class RegisterHandler : IHandler<ReqRegister, ResRegister>
    {
        private readonly AccountService _accountService;
        public RegisterHandler(IServiceProvider serviceProvider)
        {
            _accountService = serviceProvider.GetRequiredService<AccountService>();
        }


        Task<ResRegister> IHandler<ReqRegister, ResRegister>.Execute(ReqRegister packet)
        {
            throw new NotImplementedException();
        }
    }
}
