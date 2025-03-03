using Opss.PrimaryAuthorityRegister.Api.Domain.Common;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using System.Text.Json.Serialization;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

public class Role : BaseAuditableEntity, IAuthenticatedUserRole
{
    public string Name { get; set; }

    public IReadOnlyCollection<UserIdentity> UserIdentities => _userIdentities;

    private List<UserIdentity> _userIdentities;

    public Role(string name) : base()
    {
        Name = name;
        _userIdentities = new List<UserIdentity>();
    }
}
