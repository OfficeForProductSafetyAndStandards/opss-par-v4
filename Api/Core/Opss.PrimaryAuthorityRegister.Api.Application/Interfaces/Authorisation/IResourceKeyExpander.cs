namespace Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;

/// <summary>
/// Used in conjunction with the <see cref="AttributeBasedResourceClaimProvider"/> a resource key expander
/// is used to take a resource key and 'expand' it such that somebody who has a claim on a resource that
/// is the parent of an item also intrinsically has that same claim on all children.
/// </summary>
public interface IResourceKeyExpander
{
    /// <summary>
    /// Given a resource key will 'expand' it, returning the resource keys that should a user have
    /// a claim to will also have a claim to the specified resource key, typically used in the case
    /// of a hierarchical permission structure.
    /// </summary>
    /// <param name="resourceKey">The resource key to 'expand'.</param>
    /// <returns>Gets the expanded set of keys for the given resource key, excluding the resource
    /// key passed as a parameter.</returns>
    IEnumerable<string> GetKeys(string resourceKey);
}