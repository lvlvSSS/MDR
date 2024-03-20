using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;

namespace MDR.Server.Samples.Middlewares;

public class MdrJsonExceptionMiddleware
{
    private ILogger<MdrJsonExceptionMiddleware> _logger;
    private RequestDelegate _next;

    public MdrJsonExceptionMiddleware(RequestDelegate next, ILogger<MdrJsonExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 这里可以自定义 http response 内容，以下仅是示例

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
        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsJsonAsync(new
        {
            Message = message,
            Success = false,
        });
    }
}