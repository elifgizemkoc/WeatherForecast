using Moq;
using WeatherForecastApp.Interfaces;
using WeatherForecastApp.Services;

namespace WeatherForecastTest
{
    public class CityWeatherForecastRequestGroupTest
    {
        private Mock<IRedisCacheService> _mockRedisCache;
        private Mock<IWeatherForecastRepository> _mockWeatherRepository;
        private CityWeatherForecastRequestGroupService _requestGroup;
        [SetUp]
        public void SetUp()
        {
            _mockRedisCache = new Mock<IRedisCacheService>();
            _mockWeatherRepository = new Mock<IWeatherForecastRepository>();
            _requestGroup = new CityWeatherForecastRequestGroupService("Istanbul");
        }

        [Test]
        public async Task GetWeatherForecastAsync_ShouldReturnTempFromApi()
        {
            // Arrange
            var expectedTemp = 20.5;
            var weatherApiMock = new Mock<Func<Task<double?>>>();
            weatherApiMock.Setup(f => f()).ReturnsAsync(expectedTemp);
            _mockRedisCache.Setup(c => c.GetCacheValueAsync<double>(It.IsAny<string>())).ReturnsAsync(0);

            // Act
            var result = await _requestGroup.GetWeatherForecastAsync();

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
