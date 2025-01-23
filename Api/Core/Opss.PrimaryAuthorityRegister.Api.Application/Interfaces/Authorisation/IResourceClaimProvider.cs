using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;

/// <summary>
/// Given a resource (e.g. an <see cref="IRequestBase"/>) will get the claims that are being demanded
/// for authorisation purposes.
/// </summary>
public interface IResourceClaimProvider
{
    /// <summary>
    /// Gets the claims that are demanded (which may be a group of 'possible' claims, details
    /// of which can be found in the <see cref="DemandedClaims"/> documentation) for access to
    /// the given resource.
    /// </summary>
    /// <param name="resource">The resource to which access is required for claims are required for.</param>
    /// <returns>The claims demanded by the specified resource.</returns>
    DemandedClaims GetDemandedClaims(object resource);
}
