using Microsoft.EntityFrameworkCore;
using NLog;
using WeatherForecastApp.Data;
using WeatherForecastApp.Interfaces;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Repository
{
    public class FavoriteCityRepository : IFavoriteCityRepository
    {
        private readonly WeatherForecastDbContext _context;
        private static Logger logger = LogManager.GetLogger("logRules");
        public FavoriteCityRepository(WeatherForecastDbContext weatherForecastDbContext)
        {
            _context = weatherForecastDbContext;
        }
        public async Task<FavoriteCity> CreateFavoriteCityAsync(FavoriteCity favoriteCity)
        {
          
            try
            {
                var addedEntity = await _context.AddAsync(favoriteCity);
                await _context.SaveChangesAsync();
                return addedEntity?.Entity;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in CreateAsync: " + ex.Message);
                throw;
            }
        }
        public async Task<User?> GetFavoriteCityAllAsync(User user)
        {
            try
            {
                var userResult = await _context.Users.Include(c => c.FavoriteCities).FirstOrDefaultAsync(i => i.Id == user.Id);
                if (userResult == null)
                {
                    logger.Warn("GetAllAsync: The user was not found. ID: " + user.Id);
                    return null;
                }
                return userResult;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in GetAllAsync: " + ex.Message);
                throw;
            }
        }
        public async Task<FavoriteCity?> DeleteFavoriteCityAsync(int id)
        {
            try
            {
                var favoriteCityModel = await _context.FavoriteCities.FirstOrDefaultAsync(x => x.Id == id);
                if (favoriteCityModel == null)
                {
                    logger.Warn("DeleteAsync: The city to be deleted was not found. ID: " + id);
                    return null;
                }
                _context.FavoriteCities.Remove(favoriteCityModel);
                await _context.SaveChangesAsync();
                return favoriteCityModel;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in DeleteAsync: " + ex.Message);
                throw;
            }
        }
        public async Task<FavoriteCity?> GetFavoriteCityByNameAsync(FavoriteCity favoriteCity)
        {
            try
            {
                var favoriteCityResult = await _context.FavoriteCities.FirstOrDefaultAsync(i => i.CityName == favoriteCity.CityName
                && i.UserId == favoriteCity.UserId);

                return favoriteCityResult;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in GetByNameAsync: " + ex.Message);
                throw;
            }
        }
    }
}
