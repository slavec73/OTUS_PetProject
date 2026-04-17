using StackExchange.Redis;

namespace VacationPlanner.Interfaces
{
    public interface ICacheService
    {
        Task SetAsync(string key, string value, Expiration ttl);
        Task<string?> GetAsync(string key);
        Task RemoveAsync(string key);
    }
}
