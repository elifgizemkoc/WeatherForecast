namespace WeatherForecastApp.Interfaces
{
    public interface IRedisCacheService
    {
        Task<bool> SetCacheValueAsync<T>(string key, T value, TimeSpan expiration);
        Task<T?> GetCacheValueAsync<T>(string key);
    }
}
