using MDR.Data.Model.Jwt;
using MDR.Infrastructure.Extensions;
using MDR.Server.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MDR.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private IOptionsMonitor<JwtTokenParameterOptions> _jwtTokenParameterOptions;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        IOptionsMonitor<JwtTokenParameterOptions> jwtTokenParameterOptions)
    {
        _logger = logger;
        _jwtTokenParameterOptions = jwtTokenParameterOptions;
    }

    [HttpGet(template: "Get")]
    public IEnumerable<WeatherForecast> Get()
    {
        Console.WriteLine(_jwtTokenParameterOptions.CurrentValue.ToJson());
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}