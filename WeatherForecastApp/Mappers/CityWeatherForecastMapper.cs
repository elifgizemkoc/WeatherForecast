using WeatherForecastApp.Dtos;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Mappers
{
    public static class CityWeatherForecastMapper
    {
        public static CityWeatherForecastDto ToDtoFromCityWeather(this CityWeatherForecast cityWeather)
        {
            return new CityWeatherForecastDto
            {
                CityName = cityWeather.CityName,
                TempC = cityWeather.TempC
            };
        }
    }
}
