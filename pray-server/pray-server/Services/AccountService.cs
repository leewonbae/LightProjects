using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using pray_server.Commons;
using pray_server.Databases.DbContexts;
using pray_server.Exceptions;
using pray_server.Extensions;
using pray_server.Helpers;
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
        private readonly AccountManager _accountManager;
        private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;

        public AccountService(AccountManager accountManager, IDbContextFactory<AccountDbContext> accountDbContextFactory)
        {
            _accountManager = accountManager;
            _accountDbContextFactory = accountDbContextFactory;
        }

        public async Task<bool> Identify(ReqIdentify req)
        {
            var loginToken = _accountManager.GetLoginTokenByPlatformToken(req.LoginPlatformType, req.PlatformToken);

            await using var accountDbContext = await _accountDbContextFactory.CreateDbContextAsync();
            var accountLinkDto = await accountDbContext.SelectAccountLinkByLoginTokenAsync(loginToken);

            return accountLinkDto == null ? false : true;
        }

        public async Task RegisterAsync(ReqRegister req)
        {
            var serverDt = ServerDateTime.Now;

            var loginToken = _accountManager.GetLoginTokenByPlatformToken(req.LoginPlatformType, req.PlatformToken);

            await using var accountDbContext = await _accountDbContextFactory.CreateDbContextAsync();
            var accountLinkDto = await accountDbContext.SelectAccountLinkByLoginTokenAsync(loginToken);

            if (accountLinkDto != null)
            {
                throw new GameServerException(E_PACKET_ERROR_CODE.ALREADY_EXISTS_ACCOUNT_LINK, $"Account link already exists for login token [{loginToken}]");
            }

            var emptyAccountDto = _accountManager.CreateEmptyAccount(serverDt, req);

            var accountId = await accountDbContext.InsertAndSelectAccountIdAsync(emptyAccountDto);

            emptyAccountDto.SetAccountId(accountId);

            // 계정 연결 정보 추가 ,

            // GamedDB 에 추가 

        }
    }
}
