using StackExchange.Redis;

namespace app.Data.Redis;

public class RedisConnection
{
    private static Lazy<ConnectionMultiplexer>? _lazyConnection;

    public static void Init(string connectionString)
    {
        _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(connectionString);
        });
    }

    public static ConnectionMultiplexer Connection =>
        _lazyConnection?.Value
        ?? throw new InvalidOperationException("Redis connection is not initialized. Call RedisConnection.Init(...) first.");
}