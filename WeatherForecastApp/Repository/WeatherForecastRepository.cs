using Microsoft.EntityFrameworkCore;
using NLog;
using WeatherForecastApp.Data;
using WeatherForecastApp.Interfaces;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Repository
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private readonly WeatherForecastDbContext _context;
        private static Logger logger = LogManager.GetLogger("logRules");
        public WeatherForecastRepository(WeatherForecastDbContext weatherForecastDbContext)
        {
            _context = weatherForecastDbContext;
        }
        public async Task<CityWeatherForecast> CreateCityWeatherForecastAsync(CityWeatherForecast cityWeather)
        {
            try
            {
                var addedEntity = await _context.AddAsync(cityWeather);
                await _context.SaveChangesAsync();
                return addedEntity.Entity;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in CreateWeatherAsync: " + ex.Message);
                throw;
            }
        }

        public async Task<CityWeatherForecast?> GetCityWeatherForecastByNameAsync(CityWeatherForecast cityWeather)
        {
            try
            {
                var cityWeatherResult = await _context.Weathers.FirstOrDefaultAsync(i => i.CityName == cityWeather.CityName);

                return cityWeatherResult;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in GetWeatherByNameAsync: " + ex.Message);
                throw;
            }
        }
    }   
}
