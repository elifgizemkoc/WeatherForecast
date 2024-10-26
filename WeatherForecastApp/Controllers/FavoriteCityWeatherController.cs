using Microsoft.AspNetCore.Mvc;
using WeatherForecastApp.Dtos;
using WeatherForecastApp.Interfaces;
using WeatherForecastApp.Mappers;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Controllers
{
    public class FavoriteCityWeatherController : Controller
    {
        private readonly IFavoriteCityRepository _weatherRepository;
        private readonly IWeatherForecastService _weatherService;
        public FavoriteCityWeatherController(IFavoriteCityRepository weatherRepository, IWeatherForecastService weatherService)
        {
            ArgumentNullException.ThrowIfNull(weatherRepository);
            ArgumentNullException.ThrowIfNull(weatherService);

            _weatherRepository = weatherRepository;
            _weatherService = weatherService;
        }

        public async Task<IActionResult> AddFavoriteCity()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFavoriteCity(CreateFavoriteCityDto createFavoriteCityDto)
        {
            if (createFavoriteCityDto.UserId == null)
            {
                ModelState.AddModelError("UserIdError", "User Id is Required!");
                return View();
            }
            if (createFavoriteCityDto.CityName == null)
            {
                ModelState.AddModelError("CityNameError", "City Name is Required!");
                return View();
            }
            var favoriteCity = createFavoriteCityDto.ToFavoriteCityFromCreateDto();
            if (await _weatherRepository.GetFavoriteCityByNameAsync(favoriteCity) != null)
            {
                ModelState.AddModelError("CityValid", "City already added!");
                return View();
            }

            await _weatherRepository.CreateFavoriteCityAsync(favoriteCity);
            TempData["SuccessMessage"] = "City added succesfully!";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFavoriteCity(int id)
        {
            if (await _weatherRepository.DeleteFavoriteCityAsync(id) == null)
            {
                TempData["ErrorMessage"] = "City could not be deleted.";
            }
            TempData["SuccessMessage"] = "City deleted successfully!";
            return RedirectToAction("ListFavoriteCity");
        }
        public async Task<IActionResult> ListFavoriteCity()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ListFavoriteCity(UserDetailDto user)
        {
            if (user.Id <= 0)
            {
                ModelState.AddModelError("UserIdError", "User Id is Required!");
                return View();
            }

            var userDetailResult = new UserDetailDto();
            userDetailResult.FavoriteCitiesDto = new List<FavoriteCityDto>();
            userDetailResult.Id = user.Id;

            var userResult = await _weatherRepository.GetFavoriteCityAllAsync(new Models.User { Id = user.Id });
           
            if (userResult == null)
            {
                TempData["UserErrorMessage"] = "User Not Found!";
                return View();
            }

            if (userResult.FavoriteCities.Count < 1)
            {
                TempData["FavoriteCityErrorMessage"] = "User Does Not Have FavoriteCity!";
                return View();
            }

            foreach (var item in userResult.FavoriteCities)
            {
                var cityWeatherForecast = new CityWeatherForecast { CityName = item.CityName };

                var weatherResult = await _weatherService.GetWeatherForecastByCityName(cityWeatherForecast);
               
                if (weatherResult == null)
                {
                    TempData["FavoriteCityTempErrorMessage"] = "TempC does not found! City:" + item.CityName;
                    return View();
                }

                userDetailResult.FavoriteCitiesDto.Add
                    (new FavoriteCityDto { CityName = item.CityName, TempC = weatherResult.TempC, FavoriteCityId = item.Id });

            }

            userDetailResult.HottiesCity = userDetailResult.FavoriteCitiesDto.OrderByDescending(x => x.TempC)
                .FirstOrDefault().CityName;
            userDetailResult.ColdestCityName = userDetailResult.FavoriteCitiesDto.OrderBy(x => x.TempC)
            .FirstOrDefault().CityName;

            return View(userDetailResult);
        }
    }
}
