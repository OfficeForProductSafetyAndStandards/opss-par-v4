using Opss.PrimaryAuthorityRegister.Api.Application.Extensions;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;

/// <summary>
/// A <see cref="IResourceClaimProvider"/> that uses attributes on the resource to be checked
/// to determine what claims are required for a given resource.
/// </summary>
/// <remarks>
/// <para>
/// A resource (e.g. message) can be attributed with <see cref="ClaimRequiredAttribute"/>s that
/// specify what claims are required, specifying the claim type, a (optionally 'templated') resource key plus
/// the right associated.
/// </para>
/// <para>
/// If no claims are to be demanded for access to a resource then one of two attributes must be used (note that this
/// behaviour is in conjunction with the <see cref="ClaimAuthorisationBehaviour"/>):
/// <list type="table">
///   <listheader>
///       <attribute>Attribute</attribute>
///       <use>Use</use>
///   </listheader>
///   <item>
///      <attribute><see cref="AllowAnonymousAttribute"/></attribute>
///      <use>Indicates that anyone, even those not logged in to a system can access the resource.</use>
///   </item>
///   <item>
///      <attribute><see cref="MustBeAuthenticatedAttribute"/></attribute>
///      <use>Indicates that any user who has been authenticated can access the resource..</use>
///   </item>
/// </list>
/// </para>
/// <para>
/// Each <see cref="ClaimRequiredAttribute"/> can have a '<see cref="ClaimRequiredAttribute.Group"/>' associated with it, 
/// allowing a resource to have a number of groups of claims such that the user only needs to fulfil one complete group 
/// to be allowed access to the resource (e.g. ('Must be a system administrator') OR ('Must be a system user' AND 
/// 'Must have access to resource X'). See the <see cref="ClaimChecker"/> for more details of the checks
/// performed, as the <see cref="AttributeBasedResourceClaimProvider"/> only provides the ability to collect
/// the claims into groups, it performs no actual checking of claims.
/// </para>
/// <para>
/// The attribute-based claim provider supports the notion of 'expanded' resource keys in support of
/// hierarchical claim structures where having a claim higher in the tree gives access to descendent
/// resources. This is achieved by taking a dependency on any number of <see cref="IResourceKeyExpander"/>
/// implementations that can take a resource key and 'expand' it up the tree. This works such that given
/// a child resource key 'AType/63638' a key expanded could expand that to 'AParentType/51215', meaning
/// a user with a claim to 'AParentType/51215' has the same claim on 'AType/63638'.
/// </para>
/// </remarks>
public class AttributeBasedResourceClaimProvider : IResourceClaimProvider
{
    private readonly IEnumerable<IResourceKeyExpander> _resourceKeyExpanders;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeBasedResourceClaimProvider"/> class.
    /// </summary>
    /// <param name="resourceKeyExpanders">
    /// The resource key expanders.
    /// </param>
    public AttributeBasedResourceClaimProvider(IEnumerable<IResourceKeyExpander> resourceKeyExpanders)
    {
        _resourceKeyExpanders = resourceKeyExpanders;
    }

    /// <summary>
    /// Gets the required claims from the given resource, looking at the <see cref="ClaimRequiredAttribute"/>s
    /// that decorate the resource's class.
    /// </summary>
    /// <param name="resource">The resource to get the demanded claims for.</param>
    /// <returns>The claims demanded for access to the given resource.</returns>
    public DemandedClaims GetDemandedClaims(object resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        EnsureResourceDoesNotHaveAllowAnonymousAttribute(resource);
        EnsureResourceHasClaimRequiredAttribute(resource);

        var attributes = resource.GetType().GetAttributes<ClaimRequiredAttribute>(true).ToArray();
        var definedSets = attributes.Where(a => a.Group != null).Select(a => a.Group).Distinct().ToArray();

        return definedSets.Length == 0
                       ? GetRequiredClaimsForNoDefinedGroup(resource, attributes)
                       : GetRequiredClaimsForMultipleGroups(resource, attributes, definedSets!);
    }

    private static void EnsureResourceDoesNotHaveAllowAnonymousAttribute(object resource)
    {
        var allowAnonymousAttributes = resource.GetType().GetAttributes<AllowAnonymousAttribute>(true);

        if (allowAnonymousAttributes.Any())
        {
            throw new InvalidOperationException(
                    "Incorrect use of AllowAnonymousAttributeException");
        }
    }

    private static void EnsureResourceHasClaimRequiredAttribute(object resource)
    {
        var claimRequiredAttributes = resource.GetType().GetAttributes<ClaimRequiredAttribute>(true);

        if (!claimRequiredAttributes.Any() && !resource.GetType().GetAttributes<MustBeAuthenticatedAttribute>(false).Any())
        {
            throw new ClaimRequiredAttributeNotFoundException(resource);
        }
    }

    private List<Claim> ExpandClaim(Claim claim)
    {
        var expandedResourceKeyValues = _resourceKeyExpanders.SelectMany(k => k.GetKeys(claim.Value.ToString()));
        var expandedResourceKeyValueTypes = _resourceKeyExpanders.SelectMany(k => k.GetKeys(claim.ValueType.ToString()));

        List<Claim> expandedClaims = new List<Claim>();

        foreach (var value in expandedResourceKeyValues)
        {
            foreach (var valueType in expandedResourceKeyValueTypes)
            {
                expandedClaims.Add(new Claim(claim.Type, value, valueType));
            }
        }

        return new[] { claim }.Concat(expandedClaims).ToList();
    }

    private DemandedClaims GetRequiredClaimsForMultipleGroups(object resource, ClaimRequiredAttribute[] attributes, IEnumerable<string> groupsFound)
    {
        var claimSetOptions = new DemandedClaims();

        foreach (var groupName in groupsFound)
        {
            var localGroup = groupName;
            var attributesForGroup = attributes.Where(a => a.Group == null || a.Group == localGroup);
            var requiredClaimSet = new SatisficingClaimSet(
                    groupName, attributesForGroup.Select(a => ExpandClaim(a.GetClaim(resource))));

            claimSetOptions.AddSatisficingClaimSet(requiredClaimSet);
        }

        return claimSetOptions;
    }

    private DemandedClaims GetRequiredClaimsForNoDefinedGroup(
            object resource, IEnumerable<ClaimRequiredAttribute> attributes)
    {
        var claimSetOptions = new DemandedClaims();
        var requiredClaimSet = new SatisficingClaimSet("Default", attributes.Select(a => ExpandClaim(a.GetClaim(resource))));

        claimSetOptions.AddSatisficingClaimSet(requiredClaimSet);

        return claimSetOptions;
    }
}
