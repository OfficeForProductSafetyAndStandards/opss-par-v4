using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Authentication;

public class UserClaimsService : IUserClaimsService
{
    private readonly IUserIdentityRepository _identityRepository;

    public UserClaimsService(IUserIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public IReadOnlyCollection<Claim> GetUserClaims(string email)
    {
        var claims = new List<Claim>();

        var user = _identityRepository.GetUserIdentiyByEmail(email);

        if(user == null) return claims;

        if(user.AuthorityUser != null)
        {
            claims.Add(new Claim(Claims.Authority, user.AuthorityUser.AuthorityId.ToString()!));
        }

        // add "Temp Demo Claims"
        claims.AddRange(new List<Claim>()
        {
            new Claim(PermissionAttribute.PermissionClaimType, $"Owner/e3e695cc-ca85-43d8-9add-aa004eea5be5", "Create"),
            new Claim(PermissionAttribute.PermissionClaimType, $"Owner/e3e695cc-ca85-43d8-9add-aa004eea5be5", "Write"),
            new Claim(PermissionAttribute.PermissionClaimType, $"Owner/e3e695cc-ca85-43d8-9add-aa004eea5be5", "Read")
        });

        return claims;
    }
}
