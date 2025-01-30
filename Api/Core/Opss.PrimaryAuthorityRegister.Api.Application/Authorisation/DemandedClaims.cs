using System.Security;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;

/// <summary>
/// Represents a list of <see cref="SatisficingClaimSet"/>s relating to a resource.
/// </summary>
/// <para>
/// When demanding claims (e.g. through the <see cref="ClaimChecker"/>) an implementation of
/// <see cref="IResourceClaimProvider"/> will construct a <see cref="DemandedClaims"/> instance,
/// adding any number of claim sets, one of which must be satisficed to provide access to the
/// resource.
/// </para>
public class DemandedClaims
{
    private readonly List<SatisficingClaimSet> satisficingClaimSets = new List<SatisficingClaimSet>();

    /// <summary>
    /// Adds a satisficing claim set, a set claims that, if satisficed, will represent access to the
    /// resource that this demanded claims will represent.
    /// </summary>
    /// <param name="satisficingClaimSet">The claim set to be added.</param>
    public void AddSatisficingClaimSet(SatisficingClaimSet satisficingClaimSet)
    {
        satisficingClaimSets.Add(satisficingClaimSet);
    }

    /// <summary>
    /// Given a resource (used for formatting of the security exception as required) and a
    /// set of issued claims, demand will check that at least one of the possible sets of
    /// claims have been fulfilled, throwing a <see cref="SecurityException"/> in the case
    /// that it has not.
    /// </summary>
    /// <param name="resource">The resource to which authorisation is demanded.</param>
    /// <param name="issuedClaims">The claims issued.</param>
    public void Demand(object resource, IEnumerable<Claim> issuedClaims)
    {
        ArgumentNullException.ThrowIfNull(resource);

        if (!satisficingClaimSets.Any(claimSet => claimSet.IsSatisfiedBy(issuedClaims)))
        {
            throw CreateSecurityException(resource, issuedClaims);
        }
    }

    /// <summary>
    /// Given the name of a <see cref="SatisficingClaimSet"/> will return that set from the
    /// possible claim sets of this instance.
    /// </summary>
    /// <param name="name">The name of the required claim set to get.</param>
    /// <returns>The required claim set instance with the specified name.</returns>
    public SatisficingClaimSet? GetRequiredClaimSet(string name)
    {
        return satisficingClaimSets.SingleOrDefault(rcs => rcs.Name == name);
    }

    private static string ConvertClaimToString(Claim c)
    {
        return string.Format("Claim: [{0}, '{1}', {2}]", c.Type, c.Value, c.ValueType);
    }

    private SecurityException CreateSecurityException(object resource, IEnumerable<Claim> currentClaims)
    {
        var resourceName = resource.GetType().Name;
        var definedSets = string.Join(" OR ", satisficingClaimSets.Select(rcs => rcs.ToString()).ToArray());
        var userClaims = string.Join(",", currentClaims.Select(ConvertClaimToString).ToArray());
        return new SecurityException(
                $"{resourceName} requires the following permissions: {definedSets}. The user currently has: {userClaims})");
    }
}