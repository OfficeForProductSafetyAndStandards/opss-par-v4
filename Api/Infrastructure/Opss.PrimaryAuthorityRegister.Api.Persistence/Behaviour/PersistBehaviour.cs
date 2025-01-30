using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Behaviour;

/// <summary>
/// Automatically save or rollback a unit of work
/// </summary>
/// <typeparam name="TRequest">The request type (where it inherits ICommandBase)</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class PersistBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommandBase
{
    private readonly IUnitOfWork _unitOfWork;

    public PersistBehaviour(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// When the request has compelted, the unitOfWork will be committed to the database.
    /// If the request throws an exception, then the unit of work will be rolled back.
    /// </summary>
    /// <param name="request">The request being executed.</param>
    /// <param name="next">The next behaviour to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The request response.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        try
        {
            var response = await next().ConfigureAwait(false);

            await _unitOfWork.Save(cancellationToken).ConfigureAwait(false);

            return response;
        }
        catch
        {
            await _unitOfWork.Rollback().ConfigureAwait(false);

            throw;
        }
    }
}
