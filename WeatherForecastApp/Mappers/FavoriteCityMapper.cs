using WeatherForecastApp.Dtos;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Mappers
{
    public static class FavoriteCityMapper
    {
        public static FavoriteCity ToFavoriteCityFromCreateDto(this CreateFavoriteCityDto createFavoriteCityDto)
        {
            return new FavoriteCity
            {
                CityName = createFavoriteCityDto.CityName.ToLower(),
                UserId = createFavoriteCityDto.UserId
            };
        }
    }
}
