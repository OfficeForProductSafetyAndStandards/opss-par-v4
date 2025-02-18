using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Authentication.TokenHandler;

public class JwtTokenHandler : IJwtTokenHandler
{
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenHandler()
    {
        _tokenHandler = new JwtSecurityTokenHandler();
    }
    public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        var principal = _tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedSecurityToken);
        validatedToken = validatedSecurityToken;
        return principal;
    }
}
