using Newtonsoft.Json;
using WeatherForecastApp.Interfaces;
using WeatherForecastApp.Models;
using NLog;
using System.Linq.Expressions;

namespace WeatherForecastApp.Services
{
    public class CityWeatherForecastRequestGroupService : ICityWeatherForecastRequestGroupService
    {
        private static Logger logger = LogManager.GetLogger("logRules");
        private readonly string _cityName;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private Task<double?>? _weatherApiTask;
        private DateTime _lastRequestTime;

        public CityWeatherForecastRequestGroupService(string cityName)
        {
            _cityName = cityName;
        }

        public async Task<double?> GetWeatherForecastAsync()
        {
            await _semaphore.WaitAsync();

            try
            {
                if (_weatherApiTask == null || (DateTime.UtcNow - _lastRequestTime).TotalSeconds > 5)
                {
                    _weatherApiTask = GetCityForecastDataFromApi(_cityName);
                    _lastRequestTime = DateTime.UtcNow;
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Error in CityWeatherForecastRequestGroupService: " + ex.Message);
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
            return _weatherApiTask.Result;
        }

        private async Task<double?> GetCityForecastDataFromApi(string cityName)
        {
            double? tempC =null;
            try
            {
                using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) })
                {
                    using var response = await httpClient.GetAsync("http://api.openweathermap.org/data/2.5/weather?q=" + cityName + "& units=metric&cnt=1&APPID=8113fcc5a7494b0518bd91ef3acc074f");
                    string apiResponse = response.Content.ReadAsStringAsync().Result;

                    var result = JsonConvert.DeserializeObject<CityWeatherForecastDetail>(apiResponse);
                    if (result != null && result.main != null)
                    {
                        tempC = result.main.temp;
                    }
                }
            }
            catch (Exception ex) {
                logger.Error(ex, "Error in GetCityForecastDataFromApi: " + ex.Message);
                throw;
            }
    
            return tempC;
        }
    }
}
