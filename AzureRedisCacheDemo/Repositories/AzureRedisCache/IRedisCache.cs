namespace AzureRedisCacheDemo.Repositories.AzureRedisCache
{
    public interface IRedisCache
    {
        T GetCacheData<T>(string key);
        bool SetCacheData<T>(string key, T value, DateTimeOffset expirationTime);
        object RemoveData(string key);
    }
}
