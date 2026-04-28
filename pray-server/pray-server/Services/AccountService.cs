using pray_server.Databases;
using pray_server.Extensions;
using pray_server.Managers;
using Snowpipe.Commons.Packets;

namespace pray_server.Services
{
    public class RegisterResult
    {

    }

    [InjectableClass(ServiceLifetime.Scoped)]
    public class AccountService
    {
        private ProductManager _productManager;
        private AccountManager _accountManager;
        public AccountService(IServiceProvider serviceProvider, GameDbContext gameDbContext)
        {
            _productManager = serviceProvider.GetRequiredService<ProductManager>();
            _accountManager = serviceProvider.GetRequiredService<AccountManager>();
        }

        public RegisterResult RegisterService(ReqRegister req)
        {
            // _productManager.InsertProducts()
            return new RegisterResult();
        }

        public async Task<RegisterResult> RegisterServiceAsync(ReqRegister req)
        {
            // _accountManager.ValidateAccountToken("test");
            // _productManager.InsertProducts()
            return new RegisterResult();
        }
    }
}
