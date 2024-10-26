using Microsoft.EntityFrameworkCore;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Data
{
    public class WeatherForecastDbContext : DbContext
    {
        public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options) : base(options)
        {
        }
        public virtual DbSet<FavoriteCity> FavoriteCities { get; set; }
        public DbSet<CityWeatherForecast> Weathers { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
