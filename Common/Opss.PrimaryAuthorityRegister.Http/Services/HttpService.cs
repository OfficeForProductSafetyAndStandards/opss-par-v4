using System.Text;
using System.Text.Json;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Factories;
using System.Net.Http.Headers;

namespace Opss.PrimaryAuthorityRegister.Http.Services;

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;

    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Provide access to SendAsync of the HttpClient without passing a command or query.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response to be extracted from the return data</typeparam>
    /// <param name="method"></param>
    /// <param name="uri"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<HttpObjectResponse<TResponse>> HttpSendAsync<TResponse>(HttpMethod method, Uri uri, object? data = null, string? bearerToken = null)
        where TResponse : class
    {
        using var request = new HttpRequestMessage(method, uri);

        if (data != null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        }

        if (!string.IsNullOrEmpty(bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        var result = await HttpObjectResponseFactory.DetermineSuccess<TResponse>(response).ConfigureAwait(false);

        return result;
    }
}
