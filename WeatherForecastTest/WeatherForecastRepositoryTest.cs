using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecastApp.Data;
using WeatherForecastApp.Models;
using WeatherForecastApp.Repository;

namespace WeatherForecastTest
{
    internal class WeatherForecastRepositoryTest
    {
        private WeatherForecastRepository _weatherRepository;
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
            _weatherRepository = new WeatherForecastRepository(_weatherDbContext);
        }

        [Test]
        public async Task CreateWeatherAsync_ShouldAddCityWeather_WhenCityWeatherIsValid()
        {
            // Arrange
            var cityWeather = new CityWeatherForecast { CityName = "Istanbul", TempC = 20.5 };

            // Act
            var result = await _weatherRepository.CreateCityWeatherForecastAsync(cityWeather);

            // Assert       
            Assert.That(result.CityName, Is.EqualTo(cityWeather.CityName));
            Assert.That(result.TempC, Is.EqualTo(cityWeather.TempC));
        }

        [Test]
        public async Task GetWeatherByNameAsync_ShouldReturnCityWeather_WhenCityWeatherExists()
        {
            // Arrange
            var cityWeather = new CityWeatherForecast { Id = 1, CityName = "Istanbul", TempC = 20.5 };
            _weatherDbContext.Weathers.Add(cityWeather);
            await _weatherDbContext.SaveChangesAsync();

            // Act
            var result = await _weatherRepository.GetCityWeatherForecastByNameAsync(cityWeather);

            // Assert
            Assert.That(result.Id, Is.EqualTo(cityWeather.Id));
            Assert.That(result.CityName, Is.EqualTo(cityWeather.CityName));
            Assert.That(result.TempC, Is.EqualTo(cityWeather.TempC));
        }

        [Test]
        public async Task GetWeatherByNameAsync_ShouldReturnNull_WhenCityWeatherDoesNotExist()
        {
            // Arrange
            var cityWeather = new CityWeatherForecast { CityName = "NonExistentCity" };

            // Act
            var result = await _weatherRepository.GetCityWeatherForecastByNameAsync(cityWeather);

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
