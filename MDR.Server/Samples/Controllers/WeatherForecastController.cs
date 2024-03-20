using System.Net.Mime;
using System.Xml.Serialization;
using MDR.Data.Model.Jwt;
using MDR.Infrastructure.Extensions;
using MDR.Server.Model.DTO;
using MDR.Server.Samples.Filters;
using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    /// 异常处理方式之一
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [Route("error")]
    public object Error()
    {
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        _logger.LogError($"Exception Handled：{exceptionHandlerPathFeature.Error}");

        var statusCode = StatusCodes.Status500InternalServerError;
        var message = exceptionHandlerPathFeature.Error.Message;

        if (exceptionHandlerPathFeature.Error is NotImplementedException)
        {
            message = "not implemented";
            statusCode = StatusCodes.Status501NotImplemented;
        }

        Response.StatusCode = statusCode;
        Response.ContentType = MediaTypeNames.Application.Json;

        return new
        {
            Message = message,
            Success = false,
        };
    }

    //[ServiceFilter<MdrExceptionFilter>]
    [MdrExceptionFilter]
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

        throw new Exception("MDR exception error");
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}