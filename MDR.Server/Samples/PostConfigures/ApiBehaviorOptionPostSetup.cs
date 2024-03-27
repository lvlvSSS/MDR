using System.Net.Mime;
using MDR.Data.Model.Dtos;
using System.Linq;
using System.Text;
using MDR.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;

namespace MDR.Server.Samples.PostConfigures;

/// <summary>
/// IConfigureOptions 后置处理器，更换默认的校验工厂，使得输出为标准的mdr返回。
/// </summary>
public class ApiBehaviorOptionPostSetup : IPostConfigureOptions<ApiBehaviorOptions>
{
    private ILogger _logger;

    public ApiBehaviorOptionPostSetup(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("Error");
    }

    public void PostConfigure(string? name, ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = actionContext =>
        {
            var problemDetailsFactory =
                actionContext.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var problemDetails =
                problemDetailsFactory.CreateValidationProblemDetails(actionContext.HttpContext,
                    actionContext.ModelState);

            var mdrResponseMessage = MdrResponseMessage.Error(
                problemDetails.Status ?? 400,
                string.Join(" | ",
                    problemDetails.Errors.ToList().Select(pair => $"{pair.Key}:{string.Join(',', pair.Value)}"))
            );
            _logger.LogError(mdrResponseMessage.ToJson());
            var result =
                new JsonResult(mdrResponseMessage) { ContentType = MediaTypeNames.Application.Json };

            return result;
        };
    }
}