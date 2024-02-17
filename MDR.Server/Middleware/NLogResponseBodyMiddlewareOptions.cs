using NLog.Common;
using Microsoft.AspNetCore.Http;
using NLog.Web.Internal;
using System.Net.Mime;

namespace MDR.Server.Middleware;

/// <summary>
/// Contains the configuration for the NLogResponseBodyMiddlewareOptions
/// </summary>
public class NLogResponseBodyMiddlewareOptions
{
    /// <summary>
    /// The default configuration
    /// </summary>
    internal static readonly NLogResponseBodyMiddlewareOptions Default = new NLogResponseBodyMiddlewareOptions();

    /// <summary>
    /// The default constructor
    /// </summary>
    public NLogResponseBodyMiddlewareOptions()
    {
        ShouldCapture = DefaultCapture;
        ShouldRetain = DefaultRetain;

        AllowContentTypes = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("application/", "json"),
                new KeyValuePair<string, string>("text/", ""),
                new KeyValuePair<string, string>("", "charset"),
                new KeyValuePair<string, string>("application/", "xml"),
                new KeyValuePair<string, string>("application/", "html")
            };
    }

    /// <summary>
    /// The maximum response body size that will be captured. Defaults to 30KB.
    /// </summary>
    /// <remarks>
    /// Since we must capture the response body on a MemoryStream first, this will use 2x the amount
    /// of memory that we would ordinarily use for the response.
    /// </remarks>
    public int MaxContentLength { get; set; } = 30 * 1024;

    /// <summary>
    /// Prefix and suffix values to be accepted as ContentTypes. Ex. key-prefix = "application/" and value-suffix = "json"
    /// The defaults are:
    ///
    /// text/*
    /// */charset
    /// application/json
    /// application/xml
    /// application/html
    /// </summary>
    public IList<KeyValuePair<string, string>> AllowContentTypes { get; set; }

    /// <summary>
    /// If this returns true, the response body will be retained
    /// for capture
    /// Defaults to true
    /// </summary>
    /// <returns></returns>
    public Predicate<HttpContext> ShouldCapture { get; set; }

    /// <summary>
    /// If this returns true, the response body will be captured
    /// Defaults to true if content length &lt;= 30KB
    /// This can be used to capture only certain content types,
    /// only certain hosts, only below a certain request body size, and so forth
    /// </summary>
    /// <returns></returns>
    public Predicate<HttpContext> ShouldRetain { get; set; }

    /// <summary>
    /// The default predicate for ShouldCaptureResponse. Returns true
    /// Since we know nothing about the response before the response is created
    /// </summary>
    private bool DefaultCapture(HttpContext context)
    {
        return true;
    }

    /// <summary>
    /// The default predicate for ShouldRetainCapture.  Returns true if content length &lt;= 30KB
    /// and if the content type is allowed
    /// </summary>
    private bool DefaultRetain(HttpContext context)
    {
        var contentLength = context?.Response?.Body.Length ?? 0;
        if (contentLength <= 0 || contentLength > MaxContentLength)
        {
            InternalLogger.Debug("NLogResponsePostedBodyMiddleware: HttpContext.Response.ContentLength={0}", contentLength);
            return false;
        }
        /* string responseContentType = context!.Response.ContentType!;
        int idx = responseContentType.IndexOf(";");
        if (idx > 0)
        {
            responseContentType = responseContentType.Substring(0, idx);
        }
        var contentTypePairs = responseContentType.Split("/");
        if (contentTypePairs.Length != 2)
            return false;

        if (!AllowContentTypes.Any(pair =>
        {
            return string.Equals(
                pair.Key.Trim(),
                contentTypePairs[0].Trim(),
                StringComparison.OrdinalIgnoreCase) &&
                string.Equals(
                    pair.Value.Trim(),
                contentTypePairs[1].Trim(),
                StringComparison.OrdinalIgnoreCase
                );
        }))
        {
            InternalLogger.Debug("NLogResponsePostedBodyMiddleware: HttpContext.Request.ContentType={0}", context?.Request?.ContentType);
            return false;
        } */

        return true;
    }
}