using Microsoft.EntityFrameworkCore;
using WeatherForecastApp.Data;
using WeatherForecastApp.Models;
using WeatherForecastApp.Repository;

namespace WeatherForecastTest
{
    public class FavoriteCityRepositoryTest
    {
        private FavoriteCityRepository _weatherRepository;
        private WeatherForecastDbContext _weatherDbContext;

        public WeatherForecastDbContext GetMemoryContext()
        {
            var options = new DbContextOptionsBuilder<WeatherForecastDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;
            return new WeatherForecastDbContext(options);

        }
        [SetUp]
        public void Setup()
        {
            _weatherDbContext = GetMemoryContext();
            _weatherRepository = new FavoriteCityRepository(_weatherDbContext);
        }

        [Test]
        public async Task CreateAsync_ShouldReturnFavoriteCity_WhenFavoriteCityIsAdded()
        {
            var favoriteCity =
                new FavoriteCity
                {
                    UserId = 2,
                    CityName = "Istanbul",
                    Id = 2
                };
            var result = _weatherRepository.CreateFavoriteCityAsync(favoriteCity);
            Assert.That(result.Result.Id, Is.EqualTo(favoriteCity.Id));
            Assert.That(result.Result.UserId, Is.EqualTo(favoriteCity.UserId));
            Assert.That(result.Result.CityName, Is.EqualTo(favoriteCity.CityName));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "TestUser",
                FavoriteCities = new List<FavoriteCity> { new FavoriteCity { UserId = 2,
                            CityName = "Istanbul",
                            Id = 2 } }
            };
            _weatherDbContext.Users.Add(user);
            await _weatherDbContext.SaveChangesAsync();

            // Act
            var result = await _weatherRepository.GetFavoriteCityAllAsync(user);

            // Assert
            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(result.Name, Is.EqualTo(user.Name));
        }

        [Test]
        public async Task GetAllAsync_ShouldLogWarning_WhenUserDoesNotExist()
        {
            // Arrange
            var user = new User { Id = 2, Name = "NonExistentUser" };

            // Act
            var result = await _weatherRepository.GetFavoriteCityAllAsync(user);

            // Assert
            Assert.That(result, Is.EqualTo(null));
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFavoriteCity_WhenCityExists()
        {
            // Arrange
            var favoriteCity =
               new FavoriteCity
               {
                   UserId = 2,
                   CityName = "Istanbul",
                   Id = 2
               };
            _weatherDbContext.FavoriteCities.Add(favoriteCity);
            await _weatherDbContext.SaveChangesAsync();

            // Act
            var result = await _weatherRepository.DeleteFavoriteCityAsync(favoriteCity.Id);

            // Assert
            Assert.That(result.Id, Is.EqualTo(favoriteCity.Id));
            Assert.That(result.UserId, Is.EqualTo(favoriteCity.UserId));
            Assert.That(result.CityName, Is.EqualTo(favoriteCity.CityName));
        }

        [Test]
        public async Task DeleteAsync_ShouldLogWarning_WhenCityDoesNotExist()
        {
            // Arrange
            var nonExistentCityId = 2;

            // Act
            var result = await _weatherRepository.DeleteFavoriteCityAsync(nonExistentCityId);

            // Assert
            Assert.That(result, Is.EqualTo(null));
        }

    
        [TearDown]
        public void TearDown()
        {
            _weatherDbContext.Database.EnsureDeleted();
            _weatherDbContext.Dispose();
        }
    }
}
