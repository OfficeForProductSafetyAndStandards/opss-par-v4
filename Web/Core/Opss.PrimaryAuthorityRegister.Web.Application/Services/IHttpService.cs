using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;
using Opss.PrimaryAuthorityRegister.Common;
using Opss.PrimaryAuthorityRegister.Web.Application.Entities;

namespace Opss.PrimaryAuthorityRegister.Web.Application.Services;

public interface IHttpService
{
    /// <summary>
    /// Used to retrieve data
    /// </summary>
    /// <typeparam name="TQuery">The type of the query being executed</typeparam>
    /// <typeparam name="TResponse">The type of data being retrieved</typeparam>
    /// <param name="client">The HTTP Client</param>
    /// <param name="uri">Uri of the API endpoint</param>
    /// <returns></returns>
    public Task<HttpObjectResponse<TResponse>> GetAsync<TQuery, TResponse>(TQuery query)
        where TQuery : IQuery<TResponse> where TResponse : class;

    /// <summary>
    /// Used when a command will return a Guid
    /// </summary>
    /// <typeparam name="TCommand">The type of the command being executed</typeparam>
    /// <param name="client">The HTTP Cient</param>
    /// <param name="uri">Uri of the API endpoint</param>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public Task<HttpObjectResponse<CreatedResponse>> PostAsync<TCommand>(TCommand command)
        where TCommand : ICommand<Guid>;

    /// <summary>
    /// Used to execute a command that doesn't create anything (i.e. no Id to be returned)
    /// </summary>
    /// <typeparam name="TCommand">The type of the command being executed</typeparam>
    /// <param name="client">The HTTP Cient</param>
    /// <param name="uri">Uri of the API endpoint</param>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public Task<HttpObjectResponse<NoContentResult>> PutAsync<TCommand>(TCommand command)
        where TCommand : ICommand;
}
