using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;

/// <summary>
/// When applied to a message indicates that the user executing the command must have
/// a given permission, a permission that is 'system-wide' and as such is not associated
/// with any particular resource.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class MustHaveRoleAttribute : ClaimRequiredAttribute
{
    /// <summary>
    /// A claim type that indicates a user has a role.
    /// </summary>
    public const string PermissionClaimType = ClaimTypes.Role;

    private readonly string role;

    /// <summary>
    /// Initializes a new instance of the <see cref="MustHaveRoleAttribute"/> class. 
    /// </summary>
    /// <param name="role">The named role that is required.</param>
    public MustHaveRoleAttribute(string role)
        : base(PermissionClaimType, role, "http://www.w3.org/2001/XMLSchema#string")
    {
        this.role = role;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MustHaveRoleAttribute"/> class. 
    /// </summary>
    /// <param name="roles">
    ///     Joins supplied roles with a | which is expanded with 
    ///     the  <see cref="MultiplePermissionResourceKeyExpander"/> 
    /// </param>
    public MustHaveRoleAttribute(params string[] roles)
        : base(PermissionClaimType, string.Join("|", roles), "http://www.w3.org/2001/XMLSchema#string")
    {
        role = string.Join("|", roles);
    }

    /// <summary>
    /// Gets the permission this attribute is representing as being required.
    /// </summary>
    public string Role
    {
        get
        {
            return role;
        }
    }

    public string[] Roles { get { return role.Split("|"); } }
}
