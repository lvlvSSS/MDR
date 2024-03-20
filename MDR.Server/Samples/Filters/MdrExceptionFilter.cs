using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MDR.Server.Samples.Filters;

public class MdrExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled)
            return;
        // todo : handle the exception.
        context.Result = new JsonResult(new { MdrException = context.Exception.Message });
        context.ExceptionHandled = true;
    }
}