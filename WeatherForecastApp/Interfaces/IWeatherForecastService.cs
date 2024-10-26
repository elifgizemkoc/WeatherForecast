using WeatherForecastApp.Models;

namespace WeatherForecastApp.Interfaces
{
    public interface IWeatherForecastService
    {
        public Task<CityWeatherForecast?> GetWeatherForecastByCityName(CityWeatherForecast cityWeather);
    }
}
