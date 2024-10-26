namespace WeatherForecastApp.Models
{
    public class FavoriteCity
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string CityName { get; set; }
    }
}
