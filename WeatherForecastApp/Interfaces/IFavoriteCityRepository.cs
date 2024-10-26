using WeatherForecastApp.Models;

namespace WeatherForecastApp.Interfaces
{
    public interface IFavoriteCityRepository
    {
        public Task<FavoriteCity> CreateFavoriteCityAsync(FavoriteCity favoriteCity);
        public Task<FavoriteCity?> GetFavoriteCityByNameAsync(FavoriteCity favoriteCity);
        public Task<User?> GetFavoriteCityAllAsync(User user);
        public Task<FavoriteCity?> DeleteFavoriteCityAsync(int id);
        
    }
}
