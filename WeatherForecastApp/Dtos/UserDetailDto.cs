namespace WeatherForecastApp.Dtos
{
    public class UserDetailDto
    {
        public int Id { get; set; }
        public List<FavoriteCityDto> FavoriteCitiesDto { get; set; }

        public string HottiesCity { get; set; }
        public string ColdestCityName { get; set; }
    }
}
