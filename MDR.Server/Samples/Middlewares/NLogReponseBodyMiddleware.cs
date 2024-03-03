using System.Text;
using Microsoft.IO;
using NLog.Common;

namespace MDR.Server.Middleware;

/// <summary>
/// This class is to intercept the HTTP pipeline and to allow additional logging of the Response Body
///
/// Usage: app.UseMiddleware&lt;NLogResponseBodyMiddleware&gt;(); where app is an IApplicationBuilder
///
/// Inject the NLogResponseBodyMiddlewareOption in the IoC if wanting to override default values for constructor
///
/// Then add {aspnet-item:variable=nlog-aspnet-response-body} in your NLog.config
///
/// This class requires the NuGet Package Microsoft.IO.RecyclableMemoryStream to build
/// and also NLog, and NLog.Web.AspNetCore NuGet Packages
/// </summary>
public class NLogResponseBodyMiddleware
{
    /// <summary>
    /// Use {aspnet-item:variable=nlog-aspnet-response-body} in your NLog.config
    /// </summary>
    public static readonly string NLogResponseBodyKey = "nlog-aspnet-response-body";

    private readonly RequestDelegate _next;

    private readonly NLogResponseBodyMiddlewareOptions _options;

    // Using this instead of new MemoryStream() is important to the performance.
    // According to the articles, this should be used as a static and not as an instance.
    // This will manage a pool of MemoryStream instead of creating a new MemoryStream every response.
    // Otherwise we will end up doing new MemoryStream 1000s of time a minute
    // This requires the NuGet Package Microsoft.IO.RecyclableMemoryStream
    private static readonly RecyclableMemoryStreamManager MemoryStreamManager = new RecyclableMemoryStreamManager();

    /// <summary>
    /// Initializes new instance of the <see cref="NLogResponseBodyMiddleware"/> class
    /// </summary>
    /// <remarks>
    /// Use the following in Startup.cs:
    /// <code>
    /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    /// {
    ///    app.UseMiddleware&lt;NLog.Web.NLogResponseBodyMiddleware&gt;();
    /// }
    /// </code>
    /// </remarks>
    public NLogResponseBodyMiddleware(RequestDelegate next, NLogResponseBodyMiddlewareOptions options)
    {
        _next = next;
        _options = options ?? NLogResponseBodyMiddlewareOptions.Default;
    }

    /// <summary>
    /// This allows interception of the HTTP pipeline for logging purposes
    /// </summary>
    /// <param name="context">The HttpContext</param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        if (ShouldCaptureResponseBody(context))
        {
            using (var memoryStream = MemoryStreamManager.GetStream())
            {
                // Save away the true response stream
                var originalStream = context.Response.Body;

                // Make the Http Context Response Body refer to the Memory Stream
                context.Response.Body = memoryStream;

                // The Http Context Response then writes to the Memory Stream
                await _next(context).ConfigureAwait(false);

                // Should we retain the response based on the content length and content type
                // By default content length should be <=30KB
                /*  if (_options.ShouldRetain(context))
                 { */
                // This next line enables NLog to log the response
                var responseBody = await GetString(memoryStream).ConfigureAwait(false);

                // Only save the response body if there is one
                if (!string.IsNullOrEmpty(responseBody))
                {
                    context.Items.Add(NLogResponseBodyKey, responseBody);
                }
                /*  } */

                // Copy the contents of the memory stream back to the true response stream
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalStream).ConfigureAwait(false);
            }
        }
        else
        {
            if (context != null)
            {
                await _next(context).ConfigureAwait(false);
            }
        }
    }

    private bool ShouldCaptureResponseBody(HttpContext context)
    {
        // Perform null checking
        if (context == null)
        {
            InternalLogger.Debug("NLogResponsePostedBodyMiddleware: HttpContext is null");
            return false;
        }

        // Perform null checking
        if (context.Response == null)
        {
            InternalLogger.Debug("NLogResponsePostedBodyMiddleware: HttpContext.Response is null");
            return false;
        }

        // Perform null checking
        if (context.Response.Body == null)
        {
            InternalLogger.Debug("NLogResponsePostedBodyMiddleware: HttpContext.Response.Body stream is null");
            return false;
        }

        // If we cannot write the response stream we cannot capture the body
        if (!context.Response.Body.CanWrite)
        {
            InternalLogger.Debug("NLogResponsePostedBodyMiddleware: HttpContext.Response.Body stream is non-writeable");
            return false;
        }

        // Use the ShouldCaptureResponse predicate in the configuration instance that takes the HttpContext as an argument
        if (!_options.ShouldCapture(context))
        {
            InternalLogger.Debug("NLogResponsePostedBodyMiddleware: _configuration.ShouldCapture(HttpContext) predicate returned false");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Convert the stream to a String for logging.
    /// If the stream is binary please do not utilize this middleware
    /// Arguably, logging a byte array in a sensible format is simply not possible.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>The contents of the Stream read fully from start to end as a String</returns>
    private async Task<string?> GetString(Stream stream)
    {
        string responseText;

        // If we cannot seek the stream we cannot capture the body
        if (!stream.CanSeek)
        {
            InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpApplication.HttpContext.Request.Body stream is non-seekable");
            return null;
        }

        // Save away the original stream position
        var originalPosition = stream.Position;

        try
        {
            // This is required to reset the stream position to the beginning in order to properly read all of the stream.
            stream.Position = 0;

            // The last argument, leaveOpen, is set to true, so that the stream is not pre-maturely closed
            // therefore preventing the next reader from reading the stream.
            // The middle three arguments are from the configuration instance
            // These default to UTF-8, true, and 1024.
            using (var streamReader = new StreamReader(
                       stream,
                       Encoding.UTF8,
                       true,
                       1024,
                       leaveOpen: true))
            {
                // This is the most straight forward logic to read the entire body
                responseText = await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            // This is required to reset the stream position to the original, in order to
            // properly let the next reader process the stream from the original point
            stream.Position = originalPosition;
        }

        // Return the string of the body
        return responseText;
    }
}