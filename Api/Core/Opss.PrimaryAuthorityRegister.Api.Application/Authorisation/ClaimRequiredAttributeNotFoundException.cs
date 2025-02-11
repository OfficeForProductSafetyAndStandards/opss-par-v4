using System.Runtime.Serialization;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;

/// <summary>
/// An exception that is thrown when getting claims using the <see cref="AttributeBasedResourceClaimProvider"/> when
/// the resource has no such attributes, nor is it decorated with the required <see cref="MustBeAuthenticatedAttribute"/>.
/// </summary>
[Serializable]
public class ClaimRequiredAttributeNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimRequiredAttributeNotFoundException"/> class.
    /// </summary>
    /// <param name="resource">
    /// The resource.
    /// </param>
    public ClaimRequiredAttributeNotFoundException(object resource)
            : base($"The expected attribute was missing from type: {(resource ?? throw new ArgumentNullException(nameof(resource))).GetType().Name}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimRequiredAttributeNotFoundException"/> class.
    /// </summary>
    public ClaimRequiredAttributeNotFoundException()
        : base("The expected attribute is missing.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimRequiredAttributeNotFoundException"/> class.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    public ClaimRequiredAttributeNotFoundException(string message)
            : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimRequiredAttributeNotFoundException"/> class.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="inner">
    /// The inner exception.
    /// </param>
    public ClaimRequiredAttributeNotFoundException(string message, Exception inner)
            : base(message, inner)
    {
    }
}
