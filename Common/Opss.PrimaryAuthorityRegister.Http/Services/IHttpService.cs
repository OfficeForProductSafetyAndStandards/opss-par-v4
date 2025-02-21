using Opss.PrimaryAuthorityRegister.Http.Entities;

namespace Opss.PrimaryAuthorityRegister.Http.Services;

public interface IHttpService
{
    /// <summary>
    /// Provide access to SendAsync of the HttpClient without passing a command or query.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response to be extracted from the return data</typeparam>
    /// <param name="method"></param>
    /// <param name="uri"></param>
    /// <param name="data">The data to pass as Json in the request</param>
    /// <param name="bearerToken">The Auth bearer token to add to the request header</param>
    /// <returns></returns>
    public Task<HttpObjectResponse<TResponse>> HttpSendAsync<TResponse>(HttpMethod method, Uri uri, object? data = null, string? bearerToken = null)
            where TResponse : class;
}
