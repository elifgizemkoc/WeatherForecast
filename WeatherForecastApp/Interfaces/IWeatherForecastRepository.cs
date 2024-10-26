using WeatherForecastApp.Models;

namespace WeatherForecastApp.Interfaces
{
    public interface IWeatherForecastRepository
    {
        public Task<CityWeatherForecast?> GetCityWeatherForecastByNameAsync(CityWeatherForecast cityWeather);
        public Task<CityWeatherForecast> CreateCityWeatherForecastAsync(CityWeatherForecast cityWeather); 
    }
}
