using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;

/// <summary>
/// A behaviour that applies authorisation to ensure the currently executing user has access
/// to perform the operation associated with the specified method, using a <see cref="IClaimChecker"/>
/// instance to perform the actual authorisation.
/// </summary>
public class ClaimAuthorisationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest
{
    private readonly IClaimChecker claimChecker;
    private readonly ClaimsPrincipal claimsPrincipal;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimAuthorisationBehaviour"/> class.
    /// </summary>
    /// <param name="claimChecker">The claim checker.</param>
    /// <param name="claimsPrincipal">The principal that should be checked for claims.</param>
    public ClaimAuthorisationBehaviour(IClaimChecker claimChecker, ClaimsPrincipal claimsPrincipal)
    {
        this.claimChecker = claimChecker;
        this.claimsPrincipal = claimsPrincipal;
    }

    /// <summary>
    /// Authorises the use of the given message by, if no <see cref="AllowAnonymousAttribute"/> attributre
    /// exists, demanding that the claims are met using the injected <see cref="IClaimChecker"/>.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        claimChecker.Demand(claimsPrincipal, request);

        return next();
    }
}
