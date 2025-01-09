using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Web.Application.Entities;
using Opss.PrimaryAuthorityRegister.Web.Application.Factories;
using System.Text;
using System.Text.Json;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;
using Opss.PrimaryAuthorityRegister.Common;

namespace Opss.PrimaryAuthorityRegister.Web.Application.Extensions;

public static class HttpClientExtensions
{
    /// <summary>
    /// Used to retrieve data
    /// </summary>
    /// <typeparam name="Q">The type of the query being executed</typeparam>
    /// <typeparam name="T">The type of data being retrieved</typeparam>
    /// <param name="client">The HTTP Client</param>
    /// <param name="uri">Uri of the API endpoint</param>
    /// <returns></returns>
    public static async Task<HttpObjectResponse<T>> GetAsync<Q, T>(this HttpClient client, Q query)
        where Q : IQuery<T> where T : class => await HttpSendAsync<Q, T>(client, HttpMethod.Get, query).ConfigureAwait(false);

    /// <summary>
    /// Used when a command will return a Guid
    /// </summary>
    /// <typeparam name="C">The type of the command being executed</typeparam>
    /// <param name="client">The HTTP Cient</param>
    /// <param name="uri">Uri of the API endpoint</param>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public static async Task<HttpObjectResponse<CreatedResponse>> PostAsync<C>(this HttpClient client, C command)
        where C : ICommand<Guid> => await HttpSendAsync<C, CreatedResponse>(client, HttpMethod.Post, command).ConfigureAwait(false);

    /// <summary>
    /// Used to execute a command that doesn't create anything (i.e. no Id to be returned)
    /// </summary>
    /// <typeparam name="C">The type of the command being executed</typeparam>
    /// <param name="client">The HTTP Cient</param>
    /// <param name="uri">Uri of the API endpoint</param>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public static async Task<HttpObjectResponse<NoContentResult>> PutAsync<C>(this HttpClient client, C command)
        where C : ICommand => await HttpSendAsync<C, NoContentResult>(client, HttpMethod.Put, command).ConfigureAwait(false);

    private static async Task<HttpObjectResponse<T>> HttpSendAsync<R, T>(this HttpClient client, HttpMethod method, object? data)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(client);
        var name = typeof(R).Name;
        using var request = new HttpRequestMessage(method, new Uri($"api?name={name}", UriKind.Relative));

        if (data != null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        }

        var response = await client.SendAsync(request).ConfigureAwait(false);

        var result = await HttpObjectResponseFactory.DetermineSuccess<T>(response).ConfigureAwait(false);

        return result;
    }
}