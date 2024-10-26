using WeatherForecastApp.Models;

namespace WeatherForecastApp.Interfaces
{
    public interface ICityWeatherForecastRequestGroupService
    {
        public Task<double?> GetWeatherForecastAsync();
    }
}
