using pray_server.Commons;
using pray_server.Databases.Models.AccountDB;
using pray_server.Databases.Models.GameDB;

namespace pray_server.Redis.Models
{
    public class AccountInfoCache
    {
        public E_LOGIN_STATUS_TYPE LoginStatusType { get; set; } = E_LOGIN_STATUS_TYPE.NONE;
        public string SessionToken { get; set; }
        public GameAccountDto GameAccountDto { get; set; }
        public long AccountId => GameAccountDto?.AccountId ?? 0;

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
