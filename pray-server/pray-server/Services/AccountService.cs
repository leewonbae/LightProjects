using GameServer.Commons;
using GameServer.Databases.DbContexts;
using GameServer.Databases.Models.GameDB;
using GameServer.Exceptions;
using GameServer.Extensions;
using GameServer.Helpers;
using GameServer.Managers;
using GameServer.Redis.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Snowpipe.Commons.Packets;

namespace GameServer.Services
{
    public class LoginResult
    {
        public GameAccountDto GameAccountDto { get; set; }
        public List<object> ProductDtoList { get; set; } // 임시로 추가 
    }

    [InjectableClass(ServiceLifetime.Scoped)]
    public class AccountService
    {
        private readonly AccountManager _accountManager;
        private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;
        private readonly IDbContextFactory<GameDbContext> _gameDbContextFactory;

        public AccountService(AccountManager accountManager,
            IDbContextFactory<AccountDbContext> accountDbContextFactory, IDbContextFactory<GameDbContext> gameDbContextFactory)
        {
            _accountManager = accountManager;
            _accountDbContextFactory = accountDbContextFactory;
            _gameDbContextFactory = gameDbContextFactory;
        }

        public async Task<bool> Identify(ReqIdentify req)
        {
            var loginToken = _accountManager.GetLoginTokenByPlatformToken(req.LoginPlatformType, req.PlatformToken);

            await using var accountDbContext = await _accountDbContextFactory.CreateDbContextAsync();
            var accountLinkDto = await accountDbContext.SelectAccountLinkByLoginTokenAsync(loginToken);

            return accountLinkDto == null ? false : true;
        }

        public async Task Register(ReqRegister req)
        {
            var serverDt = ServerDateTime.Now;

            var loginToken = _accountManager.GetLoginTokenByPlatformToken(req.LoginPlatformType, req.PlatformToken);

            await using var accountDbContext = await _accountDbContextFactory.CreateDbContextAsync();
            var accountLinkDto = await accountDbContext.SelectAccountLinkByLoginTokenAsync(loginToken);

            if (accountLinkDto != null)
            {
                throw new GameServerException(E_PACKET_ERROR_CODE.ALREADY_EXISTS_ACCOUNT_LINK, $"Account link already exists for login token [{loginToken}]");
            }

            var newAccountDto = _accountManager.CreateEmptyAccount(serverDt, req);

            var accountId = await accountDbContext.InsertAndSelectAccountIdAsync(newAccountDto);

            newAccountDto.SetAccountId(accountId);

            // 계정 연결 정보 추가 ,
            await accountDbContext.InsertAccountLinkAsync(loginToken, req.LoginPlatformType, newAccountDto.Id, serverDt);

            // 게임정보는 loginPacket 에서 처리 
        }

        public async Task<LoginResult> Login(AccountInfoCache accountInfoCache, ReqLogin req)
        {
            var serverDt = ServerDateTime.Now;

            var loginToken = _accountManager.GetLoginTokenByPlatformToken(req.LoginPlatformType, req.PlatformToken);

            await using var accountDbContext = await _accountDbContextFactory.CreateDbContextAsync();
            await using var gameDbContext = await _gameDbContextFactory.CreateDbContextAsync();

            var accountLinkDto = await accountDbContext.SelectAccountLinkByLoginTokenAsync(loginToken);
            if (accountLinkDto == null)
            {
                throw new GameServerException(E_PACKET_ERROR_CODE.NOT_FOUND_ACCOUNT, $"Not Found Account [{loginToken}]");
            }

            var accountDto = await accountDbContext.SelectAccountAsync(accountLinkDto.AccountId);
            if (accountDto == null)
            {
                throw new GameServerException(E_PACKET_ERROR_CODE.NOT_FOUND_ACCOUNT, $"Not Found AccountDto [{loginToken}] [{accountLinkDto.AccountId}]");
            }

            bool isNewUser = false;
            List<object> productList = null;
            var gameAccountDto = await gameDbContext.SelectAccountInfoAsync(accountDto.Id);
            if (gameAccountDto == null)
            {
                //최초 로그인 유저
                isNewUser = true;
                gameAccountDto = new GameAccountDto(accountDto.Id, serverDt, accountDto.Nickname);
                productList = CreateStartProductList();
            }

            //상태 변경 
            var newSessionToken = Guid.NewGuid().ToString("N");

            accountDto.SetSessionToken(newSessionToken);
            gameAccountDto.SetLastLoginDt(serverDt);

            // db 반영
            await accountDbContext.UpdateAccountAsync(accountDto);
            await gameDbContext.UpsertGameAccountAsync(gameAccountDto);
            if (isNewUser)
            {
                // 재화 추가 
            }

            // redis 반영 
            accountInfoCache.UpdateCacheInfo(accountDto, gameAccountDto);
            //레디스 매니저 반영
            return new LoginResult
            {
                GameAccountDto = gameAccountDto,
                ProductDtoList = productList
            };

        }

        private List<object> CreateStartProductList()
        {
            return new List<object>();
        }
    }
}
