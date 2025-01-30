using System.Security.Claims;
using System.Security.Principal;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Fakes;

internal class FakeClaimsPrincipal : ClaimsPrincipal
{
    public FakeClaimsPrincipal(string username, params Claim[] claims)
        : base(new GenericIdentity(username))
    {
        ((GenericIdentity)Identity).AddClaims(claims);
    }
}
