namespace Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;

/// <summary>
/// When applied to a message indicates that the user executing the command must have
/// a permission applied at the given resource key (or above if a hierarchy exists for
/// the given resource).
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class PermissionForAttribute : ClaimRequiredAttribute
{
    private readonly string _permission;
    private readonly string _resourceKeyTemplate;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionForAttribute"/> class. 
    /// Initializes a new instance of the PermissionForAttribute.
    /// </summary>
    /// <param name="permission">The named permission that is required.</param>
    /// <param name="resourceKeyTemplate">The resource key, which may contain templated variables (e.g. Site/{SiteId}).</param>
    public PermissionForAttribute(string permission, string resourceKeyTemplate)
        : base(PermissionAttribute.PermissionClaimType, resourceKeyTemplate, permission)
    {
        _permission = permission;
        _resourceKeyTemplate = resourceKeyTemplate;
    }

    /// <summary>
    /// Gets the permission this attribute is representing as being required.
    /// </summary>
    public string Permission
    {
        get
        {
            return _permission;
        }
    }

    /// <summary>
    /// Gets the resource key template representing the required resource
    /// </summary>
    public string ResourceKeyTemplate
    {
        get
        {
            return _resourceKeyTemplate;
        }
    }
}

