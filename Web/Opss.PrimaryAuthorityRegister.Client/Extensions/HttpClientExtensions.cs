using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Client.Exceptions;
using Opss.PrimaryAuthorityRegister.Client.Factories;
using Opss.PrimaryAuthorityRegister.Core.Common.Cqrs;
using Opss.PrimaryAuthorityRegister.Core.Common.Mediator;
using System.Text;
using System.Text.Json;

namespace Opss.PrimaryAuthorityRegister.Client.Extensions;

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
    public static async Task<HttpObjectResponse<T>> GetAsync<Q, T>(this HttpClient client, Uri uri, Q query)
        where Q : IQuery<T> where T : class => await HttpSendAsync<T>(client, uri, HttpMethod.Get, query).ConfigureAwait(false);

    /// <summary>
    /// Used to create entities
    /// </summary>
    /// <typeparam name="C">The type of the command being executed</typeparam>
    /// <param name="client">The HTTP Cient</param>
    /// <param name="uri">Uri of the API endpoint</param>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public static async Task<HttpObjectResponse<CreatedResponse>> PostAsync<C>(this HttpClient client, Uri uri, C command)
        where C : ICommand<Guid> => await HttpSendAsync<CreatedResponse>(client, uri, HttpMethod.Post, command).ConfigureAwait(false);

    /// <summary>
    /// Used to execute a command that doesn't create anything (i.e. no Id to be returned)
    /// </summary>
    /// <typeparam name="C">The type of the command being executed</typeparam>
    /// <param name="client">The HTTP Cient</param>
    /// <param name="uri">Uri of the API endpoint</param>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public static async Task<HttpObjectResponse<NoContentResult>> PutAsync<C>(this HttpClient client, Uri uri, C command)
        where C : ICommand => await HttpSendAsync<NoContentResult>(client, uri, HttpMethod.Put, command).ConfigureAwait(false);

    private static async Task<HttpObjectResponse<T>> HttpSendAsync<T>(this HttpClient client, Uri uri, HttpMethod method, object? data)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(client);

        using var request = new HttpRequestMessage(method, uri);

        if (data != null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        }

        var response = await client.SendAsync(request).ConfigureAwait(false);

        var result = await HttpObjectResponseFactory.DetermineSuccess<T>(response).ConfigureAwait(false);

        return result;
    }
}