namespace MDR.Infrastructure.RestEase;

//[Obsolete("Use RequestBodySerializer instead")]
public interface IRequestBodySerializer
{
    /// <summary>
    /// Serialize the given request body
    /// </summary>
    /// <param name="body">Body to serialize</param>
    /// <typeparam name="T">Type of the body to serialize</typeparam>
    /// <returns>HttpContent to assign to the request</returns>
    HttpContent SerializeBody<T>(T body);
}
