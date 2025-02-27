using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Authentication.Jwt;

public interface IJwtService
{
    string GenerateJwt(string? email);

    ClaimsPrincipal? ValidateToken(string token, out SecurityToken validatedToken);
}
