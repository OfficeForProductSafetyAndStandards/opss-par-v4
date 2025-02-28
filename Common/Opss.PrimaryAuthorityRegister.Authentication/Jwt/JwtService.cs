using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Opss.PrimaryAuthorityRegister.Authentication.Jwt;

public class JwtService : IJwtService
{
    private readonly JwtAuthConfig _jwtAuthConfig;
    private readonly IUserRoleService _userRoleService;

    public JwtService(
        IOptions<JwtAuthConfig> jwtAuthConfig, IUserRoleService userRoleService)
    {
        ArgumentNullException.ThrowIfNull(jwtAuthConfig);

        _jwtAuthConfig = jwtAuthConfig.Value;
        _userRoleService = userRoleService;
    }

    public string GenerateJwt(string? email)
    {
        var key = Encoding.UTF8.GetBytes(_jwtAuthConfig.SecurityKey);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var identity = _userRoleService.GetUserWithRolesByEmailAddress(email);

        IEnumerable<Claim> claims = identity == null ? GetDefaultClaims(email) : GetUserClaims(identity);

        var token = new JwtSecurityToken(
            issuer: _jwtAuthConfig.IssuerUri.ToString(),
            audience: _jwtAuthConfig.AudienceUri.ToString(),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtAuthConfig.MinutesUntilExpiration),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    public ClaimsPrincipal? ValidateToken(string token, out SecurityToken validatedToken)
    {
        var handler = new JwtSecurityTokenHandler();

        var validator = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtAuthConfig.IssuerUri.ToString(),

            ValidateAudience = true,
            ValidAudience = _jwtAuthConfig.AudienceUri.ToString(),

            ValidateLifetime = true, // Ensures token expiration is checked
            ClockSkew = new TimeSpan(0, 0, 0, _jwtAuthConfig.ClockSkewSeconds, 0, 0),

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthConfig.SecurityKey))
        };

        var principal = handler.ValidateToken(token, validator, out var outToken);

        validatedToken = outToken;
        return principal;
    }

    /// <summary>
    /// Return a default claimset for the user
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    private static List<Claim> GetDefaultClaims(string? email)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),                
                new Claim(JwtRegisteredClaimNames.Sid, string.Empty)
            };
        if (!string.IsNullOrEmpty(email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));
        }
        return claims;
    }

    /// <summary>
    /// Return a claimset for the user's authenticated identity
    /// </summary>
    /// <param name="email">The user's email address to retrieve roles for</param>
    /// <returns></returns>
    private static List<Claim> GetUserClaims(AuthenticatedUserIdentity identity)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, identity.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Sid, identity.Id.ToString()),
            };

        foreach (var role in identity.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        return claims;
    }
}