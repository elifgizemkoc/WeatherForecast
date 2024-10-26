namespace WeatherForecastApp.Dtos
{
    public class CreateFavoriteCityDto
    {

       
        public int? UserId { get; set; }

        public string CityName
        { get; set; }
    }
}
