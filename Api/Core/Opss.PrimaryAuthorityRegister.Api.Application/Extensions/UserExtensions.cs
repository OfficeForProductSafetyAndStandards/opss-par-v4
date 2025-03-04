using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Extensions;

public static class UserExtensions
{
    public static Guid? GetAuthorityId(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var authorityClaim = user.Claims.FirstOrDefault(c => c.Type == Claims.Authority)?.Value;
        return Guid.TryParse(authorityClaim, out var authorityId) ? authorityId : null;
    }


    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var sidClaim = user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid)?.Value;
        return Guid.TryParse(sidClaim, out var userId) ? userId : null;
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return user.Claims.Single(c => c.Type == ClaimTypes.Email).Value;
    }
}
