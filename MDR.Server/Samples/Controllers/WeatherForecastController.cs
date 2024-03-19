using MDR.Data.Model.Jwt;
using MDR.Infrastructure.Extensions;
using MDR.Server.Model.DTO;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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

    public IDistributedCache _memoryCache { get; set; }

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        IOptionsMonitor<JwtTokenParameterOptions> jwtTokenParameterOptions)
    {
        _logger = logger;
        _jwtTokenParameterOptions = jwtTokenParameterOptions;
    }

    [Route("error")]
    public void Error(HttpContext context)
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        _logger.LogError($"Exception Handled：{exceptionHandlerPathFeature.Error}");

        var statusCode = StatusCodes.Status500InternalServerError;
        var message = exceptionHandlerPathFeature.Error.Message;

        if (exceptionHandlerPathFeature.Error is NotImplementedException)
        {
            message = "not implemented";
            statusCode = StatusCodes.Status501NotImplemented;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        context.Response.WriteAsJsonAsync(new
        {
            Message = message,
            Success = false,
        });
    }

    [HttpGet(template: "Get")]
    public IEnumerable<WeatherForecast> Get()
    {
        Console.WriteLine(_jwtTokenParameterOptions.CurrentValue.ToJson());
        if (_memoryCache.Get("abc") is null)
        {
            _logger.LogInformation("no abc exists");
            _memoryCache.SetString("abc", "123",
                new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) });
        }
        else
        {
            _logger.LogInformation("abc: {0}", _memoryCache.GetString("abc"));
        }

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}