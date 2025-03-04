using Opss.PrimaryAuthorityRegister.Api.Domain.Common;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

public class AuthorityUser : BaseAuditableEntity
{
    public UserIdentity? UserIdentity { get; private set; }
    public Guid? UserIdentityId { get; private set; }
    public Authority? Authority { get; private set; }
    public Guid? AuthorityId { get; private set; }

    public AuthorityUser() :base()
    { }

    public AuthorityUser(Guid userIdentityId, Guid authorityId) : base()
    {
        UserIdentityId = userIdentityId;
        AuthorityId = authorityId;
    }
}
