using GameServer.Databases.Models.AccountDB;
using GameServer.Databases.Models.GameDB;
using Snowpipe.Commons;

namespace GameServer.Redis
{
    public class AccountInfoCache
    {
        public E_LOGIN_STATUS_TYPE LoginStatusType { get; set; } = E_LOGIN_STATUS_TYPE.NONE;
        public string SessionToken { get; set; }
        public GameAccountDto GameAccountDto { get; set; }
        public long AccountId => GameAccountDto?.AccountId ?? 0;

        public static AccountInfoCache CreateCache(AccountDto accountDto, GameAccountDto gameAccountDto, E_LOGIN_STATUS_TYPE loginStatusType)
        {
            var newCache = new AccountInfoCache();
            newCache.UpdateCacheInfo(accountDto, gameAccountDto);
            newCache.SetLoginStatus(loginStatusType);

            return newCache;
        }
        public void UpdateCacheInfo(AccountDto accountDto)
        {
            SessionToken = accountDto.SessionToken ?? string.Empty;
        }

        public void UpdateCacheInfo(GameAccountDto gameAccountDto)
        {
            GameAccountDto = gameAccountDto;
        }

        public void UpdateCacheInfo(AccountDto accountDto, GameAccountDto gameAccountDto)
        {
            UpdateCacheInfo(accountDto);
            UpdateCacheInfo(gameAccountDto);
        }

        public void SetLoginStatus(E_LOGIN_STATUS_TYPE loginStatusType)
        {
            LoginStatusType = loginStatusType;
        }
    }
}
