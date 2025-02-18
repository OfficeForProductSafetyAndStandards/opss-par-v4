using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Authentication.TokenHandler;

public interface IJwtTokenHandler
{
    ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken);
}
