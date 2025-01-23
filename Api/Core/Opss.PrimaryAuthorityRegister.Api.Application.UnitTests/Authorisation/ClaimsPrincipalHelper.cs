using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation;

public static class ClaimsPrincipalHelper
{
    public static ClaimsPrincipal CreateUnauthenticatedUser()
    {
        // Create a ClaimsIdentity with no authentication type
        var claimsIdentity = new ClaimsIdentity(); // IsAuthenticated will default to false

        // Return the ClaimsPrincipal containing the unauthenticated identity
        return new ClaimsPrincipal(claimsIdentity);
    }
}
