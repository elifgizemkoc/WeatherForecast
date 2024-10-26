using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecastApp.Data;
using WeatherForecastApp.Interfaces;
using WeatherForecastApp.Models;
using WeatherForecastApp.Services;

namespace WeatherForecastTest
{
    public class WeatherForecastServiceTest
    {
        private Mock<IRedisCacheService> _mockRedisCacheService;
        private Mock<IWeatherForecastRepository> _mockWeatherRepository;
        private WeatherForecastService _weatherService;
        private WeatherForecastDbContext _dbContext;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;

        [SetUp]
        public void SetUp()
        {
            // In-memory veritabanı oluşturma
            var options = new DbContextOptionsBuilder<WeatherForecastDbContext>()
                              .UseInMemoryDatabase(databaseName: "TestDatabase")
                              .Options;
            _dbContext = new WeatherForecastDbContext(options);

            _mockRedisCacheService = new Mock<IRedisCacheService>();
            _mockWeatherRepository = new Mock<IWeatherForecastRepository>();
            _weatherService = new WeatherForecastService(_mockRedisCacheService.Object, _mockWeatherRepository.Object);
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
           
        }

        [TearDown]
        public void TearDown()
        {

            _dbContext?.Database.EnsureDeleted();
            _dbContext?.Dispose();
            //_dbContext.Dispose();
        }

        [Test]
        public async Task GetWeatherByCityName_ShouldReturnWeatherFromCache_WhenCacheIsNotEmpty()
        {
            // Arrange
            var cityWeather = new CityWeatherForecast { CityName = "Istanbul" };
            _mockRedisCacheService.Setup(r => r.GetCacheValueAsync<double>(It.IsAny<string>()))
                                  .ReturnsAsync(20.5);

            // Act
            var result = await _weatherService.GetWeatherForecastByCityName(cityWeather);

            //// Assert
            Assert.That(20.5, Is.EqualTo(result.TempC));
        }

        [Test]
        public async Task GetWeatherByCityName_ShouldReturnWeatherFromDb_WhenCacheIsEmpty()
        {
            // Arrange
            var cityWeather = new CityWeatherForecast { CityName = "Ankara" };
            _mockRedisCacheService.Setup(r => r.GetCacheValueAsync<double>(It.IsAny<string>()))
                                  .ReturnsAsync(0);
            _mockWeatherRepository.Setup(r => r.GetCityWeatherForecastByNameAsync(It.IsAny<CityWeatherForecast>()))
                                  .ReturnsAsync(new CityWeatherForecast { CityName = "Ankara", TempC = 25.0 });

            // Act
            var result = await _weatherService.GetWeatherForecastByCityName(cityWeather);

            // Assert
            Assert.That(25.0, Is.EqualTo(result.TempC));
        }

        [Test]
        public async Task GetWeatherByCityName_ShouldReturnWeatherFromApi_WhenCacheAndDbAreEmpty()
        {
            // Arrange
            var cityWeather = new CityWeatherForecast { CityName = "Izmir" };
            HttpClient _httpClient = new HttpClient(new FakeHttpMessageHandler());
            _mockRedisCacheService.Setup(r => r.GetCacheValueAsync<double>(It.IsAny<string>()))
                                  .ReturnsAsync(0);
            _mockWeatherRepository.Setup(r => r.GetCityWeatherForecastByNameAsync(It.IsAny<CityWeatherForecast>()))
                                  .ReturnsAsync((CityWeatherForecast)null);

            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(_httpClient);

            //// Act
            var result = await _weatherService.GetWeatherForecastByCityName(cityWeather);

            // Assert
            Assert.That(result, Is.Not.Null);

        }

        public class FakeHttpMessageHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"main\":{\"temp\":30.0}}")
                };
                return Task.FromResult(response);
            }
        }
    }
}

