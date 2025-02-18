using System.Text;
using System.Text.Json;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;
using Opss.PrimaryAuthorityRegister.Common;
using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Http.Factories;
using System.Xml.Linq;
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
    /// Used to retrieve data
    /// </summary>
    /// <typeparam name="TQuery">The type of the query being executed</typeparam>
    /// <typeparam name="TResponse">The type of data being retrieved</typeparam>
    /// <param name="query">The query object</param>
    /// <returns></returns>
    public async Task<HttpObjectResponse<TResponse>> GetAsync<TQuery, TResponse>(TQuery query)
        where TQuery : IQuery<TResponse>
        where TResponse : class => await HttpSendAsync<TQuery, TResponse>(HttpMethod.Get, query).ConfigureAwait(false);

    /// <summary>
    /// Used when a command will return a Guid
    /// </summary>
    /// <typeparam name="TCommand">The type of the command being executed</typeparam>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public async Task<HttpObjectResponse<CreatedResponse>> PostAsync<TCommand>(TCommand command)
        where TCommand : ICommand<Guid> => await HttpSendAsync<TCommand, CreatedResponse>(HttpMethod.Post, command).ConfigureAwait(false);

    /// <summary>
    /// Used to execute a command that doesn't create anything (i.e. no Id to be returned)
    /// </summary>
    /// <typeparam name="TCommand">The type of the command being executed</typeparam>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public async Task<HttpObjectResponse<NoContentResult>> PutAsync<TCommand>(TCommand command)
        where TCommand : ICommand => await HttpSendAsync<TCommand, NoContentResult>(HttpMethod.Put, command).ConfigureAwait(false);

    private async Task<HttpObjectResponse<TResponse>> HttpSendAsync<TRequest, TResponse>(HttpMethod method, object? data)
        where TResponse : class
    {
        var uri = new Uri($"api?name={typeof(TRequest).Name}", UriKind.Relative);
        return await HttpSendAsync<TResponse>(method, uri, data).ConfigureAwait(false);
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
