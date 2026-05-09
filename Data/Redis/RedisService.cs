using StackExchange.Redis;
using System.Text.Json;

namespace app.Data.Redis;

public class RedisService
{
    private readonly IDatabase _db;

    public RedisService()
    {
        _db = RedisConnection.Connection.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        if (expiry.HasValue)
        {
            await _db.StringSetAsync(key, json, expiry.Value);
            return;
        }

        await _db.StringSetAsync(key, json);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);

        if (value.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
    {
        return await _db.KeyTimeToLiveAsync(key);
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        return await _db.KeyExistsAsync(key);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}
