using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Authentication.ServiceProviders;

public static class UserClaimsServiceProvider
{
    public static ClaimsPrincipal BuildClaims(IServiceProvider provider)
    {
        var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
        var userClaimService = provider.GetRequiredService<IUserClaimsService>();
        var user = httpContextAccessor.HttpContext?.User;

        return BuildUserClaims(user, userClaimService);
    }

    /// <summary>
    /// Takes the user's claims provided by the bearer token and appends user claims.
    /// </summary>
    /// <param name="claimsPrincipal">The logged in user</param>
    /// <param name="userClaimService">Service to provide user claims</param>
    /// <returns>An updated claims principal containing claims provided by the user claims service</returns>
    private static ClaimsPrincipal BuildUserClaims(ClaimsPrincipal? user, IUserClaimsService userClaimService)
    {
        ArgumentNullException.ThrowIfNull(user);

        var claimsIdentity = new ClaimsIdentity(
                                                Array.Empty<Claim>(),
                                                "Bearer",
                                                ClaimTypes.Name,
                                                ClaimTypes.Role
                                            );

        claimsIdentity.AddClaims(user.Claims);

        var email = user.Claims.SingleOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (email != null)
        {
            var claims = userClaimService.GetUserClaims(email.Value);
            claimsIdentity.AddClaims(claims);
        }

        var principal = new ClaimsPrincipal(claimsIdentity);

        return principal;
    }
}