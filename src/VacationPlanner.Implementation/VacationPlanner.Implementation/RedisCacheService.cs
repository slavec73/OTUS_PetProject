using StackExchange.Redis;
using VacationPlanner.Interfaces;

namespace VacationPlanner.Implementation
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task SetAsync(string key, string value, Expiration ttl)
        {
            await _db.StringSetAsync(key, value, ttl);
        }

        public async Task<string?> GetAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }

    public class RedisOptions
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
    }
}
