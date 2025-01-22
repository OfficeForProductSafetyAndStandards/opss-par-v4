using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation;

public static class ClaimsPrincipalHelper
{
    public static ClaimsPrincipal CreateAuthenticatedUser(
        string authenticationType = "Bearer", 
        string userName = "TestUser", 
        params Claim[] additionalClaims)
    {
        // Create a ClaimsIdentity with authentication type and claims
        var claimsIdentity = new ClaimsIdentity(
            additionalClaims ?? Array.Empty<Claim>(), 
            authenticationType, 
            ClaimTypes.Name, 
            ClaimTypes.Role 
        );

        // Optionally add a name claim
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, userName));

        // Return the authenticated ClaimsPrincipal
        return new ClaimsPrincipal(claimsIdentity);
    }

    public static ClaimsPrincipal CreateUnauthenticatedUser()
    {
        // Create a ClaimsIdentity with no authentication type
        var claimsIdentity = new ClaimsIdentity(); // IsAuthenticated will default to false

        // Return the ClaimsPrincipal containing the unauthenticated identity
        return new ClaimsPrincipal(claimsIdentity);
    }
}
