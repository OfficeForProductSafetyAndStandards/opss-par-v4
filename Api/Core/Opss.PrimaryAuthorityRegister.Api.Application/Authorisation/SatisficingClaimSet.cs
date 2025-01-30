using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;

/// <summary>
/// Represents a claim set that satisfice access to a resource.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="SatisficingClaimSet"/> is used in conjunction with <see cref="DemandedClaims"/>, 
/// representing one of potentially many claim sets that could be satisficed to grant
/// access to a resource. 
/// </para>
/// <para>
/// To support the hierarchical nature of many systems each claim that is marked as required
/// is actually a list of claims that could be fulfilled, so that for example a claim required
/// on 'AChildType/15454' could be fulfilled by having a claim on its parent 'AParentType/8451'.
/// </para>
/// </remarks>
public class SatisficingClaimSet
{
    /// <summary>
    /// If used by a demanded claim in the resource field indicates that any claim a user has
    /// for the claim type and right will pass (e.g. a user with 'Permission', 'View' on resource 'Site/123'
    /// will fulfil a claim demand of 'Permission', 'View', on resource '*').
    /// </summary>
    private const string AnyResource = "*";

    private readonly string name;

    private readonly IEnumerable<IEnumerable<Claim>> expandedClaims;

    /// <summary>
    /// Initializes a new instance of the <see cref="SatisficingClaimSet"/> class.
    /// </summary>
    /// <param name="name">
    /// The name of this claim set.
    /// </param>
    /// <param name="expandedClaims">
    /// The expanded claims.
    /// </param>
    public SatisficingClaimSet(string name, IEnumerable<IEnumerable<Claim>> expandedClaims)
    {
        this.name = name;
        this.expandedClaims = expandedClaims.ToArray();
    }

    /// <summary>
    /// Gets the name of the satisficing claim set.
    /// </summary>
    public string Name { get { return name; } }

    /// <summary>
    /// Returns a value indicating whether this claim set is satisfied by the
    /// supplied list of <see cref="Claim"/>s.
    /// </summary>
    /// <param name="claims">The claims to be checked.</param>
    /// <returns>Whether all claims have been satisfied.</returns>
    public bool IsSatisfiedBy(IEnumerable<Claim> claims)
    {
        return expandedClaims.All(c => DoExpandedClaimsSatisfy(c, claims));
    }

    /// <summary>
    /// Gets the string representation of this satisficing claim set, including its name and
    /// all claims that must be satisficed.
    /// </summary>
    /// <returns>The string representation of the satisficing claim set.</returns>
    public override string ToString()
    {
        return "[" + Name + ": "
               + string.Join(" AND ", expandedClaims.Select(ConvertExpandedClaimSetToString).ToArray()) + "]";
    }

    private static bool DoExpandedClaimsSatisfy(IEnumerable<Claim> expandedClaims, IEnumerable<Claim> claims)
    {
        return expandedClaims.Any(rc => claims.Any(availableClaim => ClaimsAreEqual(rc, availableClaim)));
    }

    private static bool ClaimsAreEqual(Claim demandedClaim, Claim availableClaim)
    {
        return availableClaim.Type.Equals(demandedClaim.Type) &&
               availableClaim.ValueType.Equals(demandedClaim.ValueType) &&
               (availableClaim.Value.Equals(demandedClaim.Value) || demandedClaim.Value.Equals(AnyResource) || availableClaim.Value.Equals(AnyResource));
    }

    private static string ConvertClaimToString(Claim c)
    {
        return string.Format("[{0}, '{1}', {2}]", c.Type, c.Value, c.ValueType);
    }

    private static string ConvertExpandedClaimSetToString(IEnumerable<Claim> claims)
    {
        return "{" + string.Join(" OR ", claims.Select(ConvertClaimToString).ToArray()) + "}";
    }
}
