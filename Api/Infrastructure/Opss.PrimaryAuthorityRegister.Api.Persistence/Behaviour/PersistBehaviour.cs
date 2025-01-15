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
    private readonly IUnitOfWork unitOfWork;

    public PersistBehaviour(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        try
        {
            var response = await next().ConfigureAwait(false);

            await unitOfWork.Save(cancellationToken).ConfigureAwait(false);

            return response;
        }
        catch
        {
            await unitOfWork.Rollback().ConfigureAwait(false);

            throw;
        }
    }
}
