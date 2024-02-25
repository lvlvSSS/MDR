using MDR.Infrastructure.Log.Implementation;
using MDR.Infrastructure.RestEase;
using MDR.Infrastructure.RestEase.Common;
using Newtonsoft.Json;

namespace MDR.Infrastructure.Test
{
    public class RestEaseTest
    {
        [Fact]
        public void TestRestEase()
        {
            var client = new RestClient("http://localhost:5000/")
            {
                JsonSerializerSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore, Formatting = Formatting.Indented }
            }.For<IWeatherForecast>();
            var result = client.GetWeatherForecasts().Result;
            new NLog4Logging().Info(result);
        }
    }

    public interface IWeatherForecast
    {
        [Get("weatherforecast/Get")]
        Task<List<WeatherForecast>> GetWeatherForecasts();
    }

    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF { get; set; }

        public string? Summary { get; set; }
    }

}