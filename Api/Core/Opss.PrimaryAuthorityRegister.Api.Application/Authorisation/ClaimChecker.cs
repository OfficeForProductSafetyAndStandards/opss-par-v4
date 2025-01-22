using Opss.PrimaryAuthorityRegister.Api.Application.Extensions;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;

using System.Security;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;

public class ClaimChecker : IClaimChecker
{
    private readonly IResourceClaimProvider _resourceClaimProvider;

    public ClaimChecker(IResourceClaimProvider resourceClaimProvider)
    {
        _resourceClaimProvider = resourceClaimProvider;
    }

    /// <summary>
    /// Given a resource will demand that the given identity has the necessary claims to use the resource.
    /// </summary>
    /// <remarks>
    /// What a resource represents is not defined, meaning any application that makes use of a ClaimChecker can determine
    /// how the claims required are defined, although a common scenario and one used in particular in the
    /// Command and Query pipelines is for the resource to be a message that has been decorated with 
    /// attributes to identify the necessary claims to execute said message.
    /// </remarks>
    /// <param name="principal">The principal that wishes to have access to the specified resource.</param>
    /// <param name="resource">The resource that is to be checked. Must be decorated with an AuthrosationAttributeBaseAttribute.</param>

    public void Demand(ClaimsPrincipal principal, object resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(principal);

        if (!resource.GetType().HasAttribute<AuthrosationAttributeBaseAttribute>(true))
        {
            throw new ClaimRequiredAttributeNotFoundException("The resource must be decorated by an Authorisation Attribute");
        }

        if (resource.GetType().HasAttribute<AllowAnonymousAttribute>(true))
        {
            return;
        }
        
        if (principal.Identity == null || !principal.Identity.IsAuthenticated)
        {
            throw new SecurityException(
                    "You must be authenticated for this resource");
        }
        
        var demandedClaims = _resourceClaimProvider.GetDemandedClaims(resource);
        var currentClaims = principal.Claims;
        
        demandedClaims.Demand(resource, currentClaims);
    }
}
