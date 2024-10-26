using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecastApp.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace WeatherForecastTest
{
    public class RedisCacheServiceTest
    {
        private RedisCacheService _redisCacheService;
        private Mock<IConnectionMultiplexer> _redisMock;
        private Mock<IDatabase> _databaseMock;

        [SetUp]
        public void Setup()
        {

            _redisMock = new Mock<IConnectionMultiplexer>();
            _databaseMock = new Mock<IDatabase>();
            _redisMock.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_databaseMock.Object);
            _redisCacheService = new RedisCacheService(_redisMock.Object);
        }


        [Test]
        public async Task GetCachedDataAsync_ShouldReturnCachedData()
        {
            //// Arrange
            TimeSpan timeSpan = TimeSpan.FromSeconds(100);
            string key = "ece";
            int value = 3;
            //// Act
            _databaseMock.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(value);

            //// Assert

            Assert.That(_redisCacheService.GetCacheValueAsync<int>(key).Result, Is.EqualTo(value));
        }

        [Test]
        public async Task SetCachedDataAsync_ShouldSetCachedData()
        {
            // Arrange
            string key = "test-key";
            int value = 13;
            TimeSpan timeSpan = TimeSpan.FromSeconds(100);

            // Act
            await _redisCacheService.SetCacheValueAsync(key, value, timeSpan);
            // Assert
            _databaseMock.Verify(db => db.StringSetAsync(key, value, timeSpan, false, When.Always, CommandFlags.None), Times.Once);
        }

        [Test]
        public void SetCachedDataAsync_WhenKeyIsNull_ShouldThrowsException()
        {// Arrange
            string key = null;
            string value = "gizem";
            TimeSpan timeSpan = TimeSpan.FromSeconds(100);
            // Assert
            Assert.That(() => _redisCacheService.SetCacheValueAsync<string>(key, value, timeSpan), Throws.Exception);

        }
        [Test]
        public void SetCachedDataAsync_WhenValueIsNull_ShouldThrowsException()
        {
            string key = "1";
            string value = null;
            TimeSpan timeSpan = TimeSpan.FromSeconds(100);
            Assert.That(() => _redisCacheService.SetCacheValueAsync<string>(key, value, timeSpan), Throws.Exception);
        }

        [Test]
        public void GetCachedDataAsync_WhenKeyIsNull_ShouldThrowsException()
        {
            string key = null;
            Assert.That(() => _redisCacheService.GetCacheValueAsync<string>(key), Throws.Exception);
            //Assert.Throws<ArgumentNullException>(
            //    () => _cacheService.GetCacheValueAsync<string>(key));
        }

        [Test]
        public void GetCachedDataAsync_WhenFailToGetCacheValue_ShouldReturnsEmpty()
        {
            string key = "sarı";
            Assert.That(_redisCacheService.GetCacheValueAsync<string>(key).Result, Is.EqualTo(null));
        }
    }
}

