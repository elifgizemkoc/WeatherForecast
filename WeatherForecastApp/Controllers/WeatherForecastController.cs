using Microsoft.AspNetCore.Mvc;
using WeatherForecastApp.Dtos;
using WeatherForecastApp.Interfaces;
using WeatherForecastApp.Mappers;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Controllers
{
    public class WeatherForecastController : Controller
    {
        private readonly IWeatherForecastService _weatherForecastService;
        public WeatherForecastController(IWeatherForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string[] cityNames)
        {
            cityNames = cityNames.Where(city => !string.IsNullOrEmpty(city)).ToArray();
            var cityWeatherInfos = new List<CityWeatherForecastDto>();
           

            foreach (var cityName in cityNames)
            {
                var cityWeather = new CityWeatherForecast
                {
                    CityName = cityName
                };

                var result = await _weatherForecastService.GetWeatherForecastByCityName(cityWeather);
                if (result == null)
                {
                    TempData["CityErrorMessage"] = "City is Not Found! City: " + cityName;
                    return View();
                }
                var favoriteCityDto = result.ToDtoFromCityWeather();
                cityWeatherInfos.Add(favoriteCityDto);
            }

            return View(cityWeatherInfos);
        }
    }
}
