using StackExchange.Redis;

namespace MonAppK8s.Services;

public class RedisService(IConnectionMultiplexer redis) : IRedisService
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task SetValueAsync(string key, string value)
    {
        await _db.StringSetAsync(key, value);
    }

    public async Task<string?> GetValueAsync(string key)
    {
        return await _db.StringGetAsync(key);
    }
}
