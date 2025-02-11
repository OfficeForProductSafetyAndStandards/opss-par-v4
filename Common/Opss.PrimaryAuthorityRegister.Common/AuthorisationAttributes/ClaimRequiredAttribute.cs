using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;

/// <summary>
/// An attribute that decorates a resource to indicate that the user must have a 
/// claim to the provided resource by the resource's Id.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Performance", 
    "CA1813:Avoid unsealed attributes", 
    Justification = "Claim Required Attribute is inherited by several other claim based attributes, so can't be sealed")]
public class ClaimRequiredAttribute : AuthrosationAttributeBaseAttribute
{
    private static readonly Regex ResourceKeyPropertyRegex = new("{([^}]+)}", RegexOptions.Compiled);

    private readonly string _claimType;
    private readonly string _valueTemplate;
    private readonly string _valueType;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimRequiredAttribute"/> class that will be
    /// part of the default group of claims demanded.
    /// </summary>
    /// <param name="claimType">The claim type.</param>
    /// <param name="valueTemplate">The resource key template.</param>
    /// <param name="valueType">The right.</param>
    public ClaimRequiredAttribute(string claimType, string valueTemplate, string valueType)
    {
        _claimType = claimType;
        _valueTemplate = valueTemplate;
        _valueType = valueType;
    }

    /// <summary>
    /// Gets or sets the group this attribute belongs to, which may be <c>null</c>.
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// Gets the claim type this attribute represents.
    /// </summary>
    public string ClaimType { get { return _claimType; } }

    /// <summary>
    /// Gets the value key template this attribute represents.
    /// </summary>
    public string ValueTemplate { get { return _valueTemplate; } }

    /// <summary>
    /// Gets the value type this attribute represents.
    /// </summary>
    public string ValueType { get { return _valueType; } }

    /// <summary>
    /// Gets the claims that decorate the type of the given object.
    /// </summary>
    /// <param name="resource">The resource on which this attribute resides.</param>
    /// <returns>The claims that this attribute demands.</returns>
    public Claim GetClaim(object resource)
    {
        var value = ResourceKeyPropertyRegex.Replace(
                                                    ValueTemplate,
                                                    m =>
                                                    {
                                                        var propertyName = m.Value.Substring(1, m.Value.Length - 2);
                                                        var property = resource.GetType().GetProperty(propertyName);

                                                        if (property == null)
                                                        {
                                                            throw new InvalidOperationException(
                                                                    "{resource.GetType()}'s ClaimRequiredAttribute missing property: {propertyName}");
                                                        }

                                                        var propertyValue = property.GetValue(resource, null);

                                                        return propertyValue?.ToString() ?? "[null]";
                                                    });

        return new Claim(ClaimType, value, ValueType);
    }

}