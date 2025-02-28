using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Opss.PrimaryAuthorityRegister.Authentication.Jwt;


namespace Opss.PrimaryAuthorityRegister.Authentication.Middleware;

public class TokenToClaimsMiddleware
{
    private readonly RequestDelegate _next;

    public TokenToClaimsMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context,
        IJwtService jwtService)
    {
        string authorizationHeader = context.Request.Headers["Authorization"];

        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            
            var principal = jwtService.ValidateToken(token, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtToken)
            {
                context.User = principal;
            }
    }

        await _next(context);
    }
}
