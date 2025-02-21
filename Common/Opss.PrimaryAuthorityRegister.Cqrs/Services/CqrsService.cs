using Opss.PrimaryAuthorityRegister.Http.Entities;
using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Cqrs;
using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Http.Services;

public class CqrsService : ICqrsService
{
    private readonly IHttpService _httpService;

    public CqrsService(IHttpService httpService)
    {
        _httpService = httpService;
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
        return await _httpService.HttpSendAsync<TResponse>(method, uri, data).ConfigureAwait(false);
    }
}
