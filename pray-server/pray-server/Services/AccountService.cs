using GameServer.Commons;
using GameServer.Databases.DbContexts;
using GameServer.Databases.Models.AccountDB;
using GameServer.Databases.Models.GameDB;
using GameServer.Exceptions;
using GameServer.Extensions;
using GameServer.Helpers;
using GameServer.Managers;
using GameServer.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Snowpipe.Commons.Packets;

namespace GameServer.Services
{
    public class LoginResult
    {
        public GameAccountDto GameAccountDto { get; set; }
        public string SessionToken { get; set; }
        public List<object> ProductDtoList { get; set; } // 임시로 추가 
    }

    [InjectableClass(ServiceLifetime.Scoped)]
    public class AccountService
    {
        private readonly AccountManager _accountManager;
        private readonly RedisRepository _redisRepository;
        private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;
        private readonly IDbContextFactory<GameDbContext> _gameDbContextFactory;

        public AccountService(AccountManager accountManager, RedisRepository redisRepository,
            IDbContextFactory<AccountDbContext> accountDbContextFactory, IDbContextFactory<GameDbContext> gameDbContextFactory)
        {
            _accountManager = accountManager;
            _redisRepository = redisRepository;

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

            var newAccountDto = AccountDto.CreateDto(serverDt, req);

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

            bool isFisrtLogin = false;
            var gameAccountDto = await gameDbContext.SelectAccountInfoAsync(accountDto.Id);
            if (gameAccountDto == null)
            {
                gameAccountDto = GameAccountDto.CreateDto(accountDto, serverDt);
                isFisrtLogin = true;
            }

            // oldSession확인
            if (!isFisrtLogin)
            {
                var oldSessionInfo = await _redisRepository.GetAccountInfoCacheAsync(accountDto.SessionToken);
                if (oldSessionInfo != null)
                {
                    oldSessionInfo.SetLoginStatus(E_LOGIN_STATUS_TYPE.DUPLICATED);
                    await _redisRepository.SetAccountInfoCacheAsync(accountDto.SessionToken, oldSessionInfo);
                }
            }

            var newSessionToken = _accountManager.UpdateLoginInfo(accountDto, gameAccountDto, serverDt);

            // db 반영
            await accountDbContext.UpdateAccountAsync(accountDto);
            await gameDbContext.UpsertGameAccountAsync(gameAccountDto);

            // redis 반영 
            var newAccountInfoCache = AccountInfoCache.CreateCache(accountDto, gameAccountDto, E_LOGIN_STATUS_TYPE.LOGINED);
            await _redisRepository.SetAccountInfoCacheAsync(accountDto.SessionToken, newAccountInfoCache);

            return new LoginResult
            {
                SessionToken = newSessionToken,
                GameAccountDto = gameAccountDto,
            };

        }
    }
}
