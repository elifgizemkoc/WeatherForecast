using NLog;
using System.Collections.Concurrent;
using WeatherForecastApp.Interfaces;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {

        private static ConcurrentDictionary<string, ICityWeatherForecastRequestGroupService> _cityWeatherForecastGroups = new();
        private readonly IRedisCacheService _redisCache;
        private readonly IWeatherForecastRepository _weatherRepository;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);
        private static Logger logger = LogManager.GetLogger("logRules");
        public WeatherForecastService(IRedisCacheService redisCacheService, IWeatherForecastRepository weatherRepository)
        {
            _redisCache = redisCacheService;
            _weatherRepository = weatherRepository;
        }
      
        public async Task<CityWeatherForecast?> GetWeatherForecastByCityName(CityWeatherForecast cityWeather)
        {
            var cacheKey = cityWeather.CityName;
        
            try
            {    //Get data from cache first
                var cachedProduct = await _redisCache.GetCacheValueAsync<double>(cacheKey);
                if (cachedProduct != 0)
                {
                    cityWeather.TempC = cachedProduct;
                    return cityWeather;
                }

                //get data from db
                var dbResult = await _weatherRepository.GetCityWeatherForecastByNameAsync(cityWeather);

                if (dbResult != null)
                {
                    //adding data to cache
                    await _redisCache.SetCacheValueAsync(cacheKey, dbResult.TempC, CacheExpiration);
                    return dbResult;
                }

                //if cache does not contain, get data from api
                var weatherRequestGroup = _cityWeatherForecastGroups.GetOrAdd(cityWeather.CityName, new CityWeatherForecastRequestGroupService(cityWeather.CityName));
                var tempCResult = await weatherRequestGroup.GetWeatherForecastAsync();
               
                if (tempCResult == null)
                    return null;

                cityWeather.TempC = tempCResult.Value;
                //adding data to cache 
                await _redisCache.SetCacheValueAsync(cacheKey, cityWeather.TempC, CacheExpiration);
                //adding data to db
                await _weatherRepository.CreateCityWeatherForecastAsync(cityWeather);

                return cityWeather;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in GetWeatherByCityName: " + ex.Message);
                throw;
            }
        }
    }
}

