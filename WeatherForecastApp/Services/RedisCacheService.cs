using NLog;
using StackExchange.Redis;
using System.Text.Json;
using WeatherForecastApp.Interfaces;

namespace WeatherForecastApp.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private static Logger logger = LogManager.GetLogger("logRules");
        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        public async Task<bool> SetCacheValueAsync<T>(string key, T value, TimeSpan expiration)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(key);
                ArgumentNullException.ThrowIfNull(value);
                var db = _redis.GetDatabase();
                var json = JsonSerializer.Serialize(value);

                await db.StringSetAsync(key, json, expiration);
                return true;
            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex, "Error: ArgumentNullException in SetCacheValueAsync: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in SetCacheValueAsync: " + ex.Message);
                throw;
            }
        }
        public async Task<T?> GetCacheValueAsync<T>(string key)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(key);
                var db = _redis.GetDatabase();
                var json = await db.StringGetAsync(key);
                if (string.IsNullOrEmpty(json))
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex, "Error: ArgumentNullException in GetCacheValueAsync: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in GetCacheValueAsync: " + ex.Message);
                throw;
            }
        }
    }
}
