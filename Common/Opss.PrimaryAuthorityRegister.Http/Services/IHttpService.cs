using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;
using Opss.PrimaryAuthorityRegister.Common;
using Opss.PrimaryAuthorityRegister.Http.Entities;

namespace Opss.PrimaryAuthorityRegister.Http.Services;

public interface IHttpService
{
    /// <summary>
    /// Used to retrieve data
    /// </summary>
    /// <typeparam name="TQuery">The type of the query being executed</typeparam>
    /// <typeparam name="TResponse">The type of data being retrieved</typeparam>
    /// <param name="query">The query object</param>
    /// <returns></returns>
    public Task<HttpObjectResponse<TResponse>> GetAsync<TQuery, TResponse>(TQuery query)
        where TQuery : IQuery<TResponse> where TResponse : class;

    /// <summary>
    /// Used when a command will return a Guid
    /// </summary>
    /// <typeparam name="TCommand">The type of the command being executed</typeparam>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public Task<HttpObjectResponse<CreatedResponse>> PostAsync<TCommand>(TCommand command)
        where TCommand : ICommand<Guid>;

    /// <summary>
    /// Used to execute a command that doesn't create anything (i.e. no Id to be returned)
    /// </summary>
    /// <typeparam name="TCommand">The type of the command being executed</typeparam>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public Task<HttpObjectResponse<NoContentResult>> PutAsync<TCommand>(TCommand command)
        where TCommand : ICommand;

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
