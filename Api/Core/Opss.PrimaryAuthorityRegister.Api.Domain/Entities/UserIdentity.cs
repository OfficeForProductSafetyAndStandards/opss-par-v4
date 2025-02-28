using Opss.PrimaryAuthorityRegister.Api.Domain.Common;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

public class UserIdentity : BaseAuditableEntity, IAuthenticatedUserIdentity<Role>
{
    public string EmailAddress { get; set; }
    public IReadOnlyCollection<Role> Roles => _roles;

    private readonly List<Role> _roles;

    public UserIdentity(string emailAddress) : base()
    {
        EmailAddress = emailAddress;
        _roles = new List<Role>();
    }
    public UserIdentity(string emailAddress, Role role) : base()
    {
        EmailAddress = emailAddress;
        _roles = new List<Role> { role };
    }
    public UserIdentity(string emailAddress, Role[] roles) : base()
    {
        EmailAddress = emailAddress;
        _roles = new List<Role>(roles);
    }
}