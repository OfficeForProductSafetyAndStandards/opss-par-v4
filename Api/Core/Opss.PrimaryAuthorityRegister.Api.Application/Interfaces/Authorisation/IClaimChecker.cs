using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;

/// <summary>
/// A claim checker is able to take a resource (typically a message in a CQRS based system) and determine whether
/// the current user (IIdentity) has access to that resource.
/// </summary>
public interface IClaimChecker
{
    /// <summary>
    /// Given a 'resource' will demand that the current user (e.g. Thread.CurrentPrincipal) has the
    /// necessary claims to use the resource.
    /// </summary>
    /// <remarks>
    /// What a resource represents is not defined, meaning any application that makes use of a IClaimChecker can determine
    /// how the claims required are defined, although a common scenario and one used in particular in the
    /// Command and Query pipelines is for the resource to be a message that has been decorated with 
    /// attributes to identify the necessary claims to execute said message.
    /// </remarks> 
    /// <param name="principal">The principal that wishes to have access to the specified resource.</param>
    /// <param name="resource">The resource that is to be checked.</param>
    void Demand(ClaimsPrincipal principal, object resource);
}