using Opss.PrimaryAuthorityRegister.Client.Exceptions;
using Opss.PrimaryAuthorityRegister.Client.Factories;
using System.Text;
using System.Text.Json;

namespace Opss.PrimaryAuthorityRegister.Client.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpObjectResponse<T>> GetAsync<T>(this HttpClient client, Uri uri)
        where T : class => await GetAsync<T>(client, uri, null).ConfigureAwait(false);

    public static async Task<HttpObjectResponse<T>> GetAsync<T>(this HttpClient client, Uri uri, object? query)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(client);

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);

        if (query != null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json");
        }

        var response = await client.SendAsync(request).ConfigureAwait(false);

        var result = await HttpObjectResponseFactory.DetermineSuccess<T>(response).ConfigureAwait(false);

        return result;
    }

    public static async Task<HttpObjectResponse> PostAsync<T>(this HttpClient client, Uri uri, T data)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(client);

        using var request = new HttpRequestMessage(HttpMethod.Post, uri);

        if (data != null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        }

        var response = await client.SendAsync(request).ConfigureAwait(false);

        var result = await HttpObjectResponseFactory.DetermineSuccess(response).ConfigureAwait(false);

        return result;
    }
}