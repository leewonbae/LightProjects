using GameServer.Extensions;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace GameServer.Redis
{
    public enum E_REDIS_DATABASE
    {
        SYSTEM = 0,
        ACCOUNT = 1,
    }

    [InjectableClass(ServiceLifetime.Singleton)]
    public class RedisManager
    {
        private const int DAY_EXPIRE_SECONDS = 86400;

        private readonly IConnectionMultiplexer _redis;
        public RedisManager(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<AccountInfoCache?> GetAccountInfoCacheAsync(string sessionToken)
        {
            var redisKey = RedisKeyFactory.GetAccountInfoKey(sessionToken);

            var db = _redis.GetDatabase((int)E_REDIS_DATABASE.ACCOUNT);
            var result = await db.StringGetAsync(redisKey);

            return result == RedisValue.Null ? null : JsonConvert.DeserializeObject<AccountInfoCache>(result);
        }

        public async Task SetAccountInfoCacheAsync(string sessionToken, AccountInfoCache accountInfoCache)
        {
            var redisKey = RedisKeyFactory.GetAccountInfoKey(sessionToken);

            var db = _redis.GetDatabase((int)E_REDIS_DATABASE.ACCOUNT);

            await db.StringSetAsync(redisKey, JsonConvert.SerializeObject(accountInfoCache), TimeSpan.FromSeconds(DAY_EXPIRE_SECONDS));
        }
    }
}
