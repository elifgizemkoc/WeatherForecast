using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using StackExchange.Redis;
using WeatherForecastApp.Data;
using WeatherForecastApp.Interfaces;
using WeatherForecastApp.Repository;
using WeatherForecastApp.Services;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddLogging(loggingBuilders =>
//{
//    loggingBuilders.ClearProviders();
//    loggingBuilders.AddNLog();
//});

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
builder.Services.AddScoped<IFavoriteCityRepository, FavoriteCityRepository>();

var redisConfiguration = builder.Configuration.GetConnectionString("Redis");
var redis = ConnectionMultiplexer.Connect(redisConfiguration);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WeatherForecastDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WeatherForecast}/{action=Index}/{id?}");

app.Run();
