using System.Reflection;
using MDR.Infrastructure.RestEase.Implementation;

namespace MDR.Infrastructure.RestEase;

/// <summary>
/// Class containing information to construct a request from.
/// An instance of this is created per request by the generated interface implementation
/// </summary>
public interface IRequestInfo
{
    /// <summary>
    /// Gets the HttpMethod which should be used to make the request
    /// </summary>
    HttpMethod Method { get; }

    /// <summary>
    /// Gets the path which should be prepended to <see cref="Path"/>, if any
    /// </summary>
    string? BaseAddress { get; }

    /// <summary>
    /// Gets the path which should be prepended to <see cref="Path"/> unless <see cref="Path"/> starts with a '/', if any
    /// </summary>
    string? BasePath { get; }

    /// <summary>
    /// Gets the relative path to the resource to request
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Gets the CancellationToken used to cancel the request
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets a value indicating whether to suppress the exception on invalid status codes
    /// </summary>
    bool AllowAnyStatusCode { get; }

    /// <summary>
    /// Gets the query parameters to append to the request URI
    /// </summary>
    IEnumerable<QueryParameterInfo> QueryParams { get; }

    /// <summary>
    /// Gets the raw query parameter infos
    /// </summary>
    IEnumerable<RawQueryParameterInfo> RawQueryParameters { get; }

    /// <summary>
    /// Gets the parameters which should be substituted into placeholders in the Path
    /// </summary>
    IEnumerable<PathParameterInfo> PathParams { get; }

    /// <summary>
    /// Gets the values from properties which should be substituted into placeholders in the Path
    /// </summary>
    IEnumerable<PathParameterInfo> PathProperties { get; }

    /// <summary>
    /// Gets the values from properties which should be added to all query strings
    /// </summary>
    IEnumerable<QueryParameterInfo> QueryProperties { get; }

    /// <summary>
    /// Gets the values from properties which should be added as HTTP request message properties
    /// </summary>
    IEnumerable<HttpRequestMessagePropertyInfo> HttpRequestMessageProperties { get; }

    /// <summary>
    /// Gets the headers which were applied to the interface
    /// </summary>
    IEnumerable<KeyValuePair<string, string?>>? ClassHeaders { get; }

    /// <summary>
    /// Gets the headers which were applied using properties
    /// </summary>
    IEnumerable<HeaderParameterInfo> PropertyHeaders { get; }

    /// <summary>
    /// Gets the headers which were applied to the method being called
    /// </summary>
    IEnumerable<KeyValuePair<string, string?>> MethodHeaders { get; }

    /// <summary>
    /// Gets the headers which were passed to the method as parameters
    /// </summary>
    IEnumerable<HeaderParameterInfo> HeaderParams { get; }

    /// <summary>
    /// Gets information the [Body] method parameter, if it exists
    /// </summary>
    BodyParameterInfo? BodyParameterInfo { get; }

    /// <summary>
    /// Gets the MethodInfo of the interface method which was invoked
    /// </summary>
    MethodInfo MethodInfo { get; }
}