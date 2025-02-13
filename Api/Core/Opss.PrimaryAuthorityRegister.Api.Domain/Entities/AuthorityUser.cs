using Opss.PrimaryAuthorityRegister.Api.Domain.Common;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

public class AuthorityUser : BaseAuditableEntity
{
    public AuthorityUser(string email)
    {
        Email = email;
    }

    public string Email { get; set; }
}
