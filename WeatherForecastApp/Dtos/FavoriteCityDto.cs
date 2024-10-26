namespace WeatherForecastApp.Dtos
{
    public class FavoriteCityDto
    {
        private string _cityName;
        public string CityName
        {
            get => _cityName;
            set => _cityName = value.ToLower();
        }
        public double TempC { get; set; }
        public int FavoriteCityId { get; set; }
    }
}
