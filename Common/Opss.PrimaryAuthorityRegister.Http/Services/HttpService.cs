using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;
using Opss.PrimaryAuthorityRegister.Common;
using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Http.Factories;

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
    /// <param name="client">The HTTP Client</param>
    /// <param name="uri">Uri of the API endpoint</param>
    /// <returns></returns>
    public async Task<HttpObjectResponse<TResponse>> GetAsync<TQuery, TResponse>(TQuery query)
        where TQuery : IQuery<TResponse>
        where TResponse : class => await HttpSendAsync<TQuery, TResponse>(HttpMethod.Get, query).ConfigureAwait(false);

    /// <summary>
    /// Used when a command will return a Guid
    /// </summary>
    /// <typeparam name="TCommand">The type of the command being executed</typeparam>
    /// <param name="client">The HTTP Cient</param>
    /// <param name="uri">Uri of the API endpoint</param>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public async Task<HttpObjectResponse<CreatedResponse>> PostAsync<TCommand>(TCommand command)
        where TCommand : ICommand<Guid> => await HttpSendAsync<TCommand, CreatedResponse>(HttpMethod.Post, command).ConfigureAwait(false);

    /// <summary>
    /// Used to execute a command that doesn't create anything (i.e. no Id to be returned)
    /// </summary>
    /// <typeparam name="TCommand">The type of the command being executed</typeparam>
    /// <param name="client">The HTTP Cient</param>
    /// <param name="uri">Uri of the API endpoint</param>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public async Task<HttpObjectResponse<NoContentResult>> PutAsync<TCommand>(TCommand command)
        where TCommand : ICommand => await HttpSendAsync<TCommand, NoContentResult>(HttpMethod.Put, command).ConfigureAwait(false);

    private async Task<HttpObjectResponse<TResponse>> HttpSendAsync<TRequest, TResponse>(HttpMethod method, object? data)
        where TResponse : class
    {
        var name = typeof(TRequest).Name;
        using var request = new HttpRequestMessage(method, new Uri($"api?name={name}", UriKind.Relative));

        if (data != null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        }

        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        var result = await HttpObjectResponseFactory.DetermineSuccess<TResponse>(response).ConfigureAwait(false);

        return result;
    }
}
