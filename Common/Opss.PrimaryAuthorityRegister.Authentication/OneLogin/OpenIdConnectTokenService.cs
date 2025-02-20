using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Authentication.TokenHandler;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace Opss.PrimaryAuthorityRegister.Authentication.OneLogin;

public class OpenIdConnectTokenService : ITokenService
{
    private readonly JwtAuthConfig _jwtAuthConfig;
    private readonly OpenIdConnectAuthConfig _oidcAuthConfig;
    private readonly IJwtTokenHandler _tokenHandler;
    private readonly IAuthenticatedUserService _authUserService;

    public OpenIdConnectTokenService(
        IOptions<OpenIdConnectAuthConfig> oneLoginAuthConfig,
        IOptions<JwtAuthConfig> jwtAuthConfig,
        IJwtTokenHandler tokenHandler,
        IAuthenticatedUserService authUserService)
    {
        ArgumentNullException.ThrowIfNull(jwtAuthConfig);
        ArgumentNullException.ThrowIfNull(oneLoginAuthConfig);

        _jwtAuthConfig = jwtAuthConfig.Value;
        _oidcAuthConfig = oneLoginAuthConfig.Value;
        _tokenHandler = tokenHandler;
        _authUserService = authUserService;
    }

    public string GenerateJwtToken(string email)
    {
        var key = Encoding.UTF8.GetBytes(_jwtAuthConfig.SecurityKey);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        IEnumerable<Claim> claims = GetDefaultClaims(email);

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

    public async Task ValidateTokenAsync(string idToken, CancellationToken cancellationToken)
    {
        var response = await _authUserService.GetSigningKeys().ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response.StatusCode, response.Problem?.Detail);
        }

        var keys = (response.Result?.Keys) ?? throw new AuthenticationException("An empty key set was returned");

        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = _oidcAuthConfig.IssuerUri.ToString(),
            ValidAudience = _oidcAuthConfig.ClientId,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) => keys,
            ClockSkew = TimeSpan.FromSeconds(_oidcAuthConfig.ClockSkewSeconds)
        };

        _tokenHandler.ValidateToken(idToken, tokenValidationParameters, out SecurityToken _);
    }

    private static List<Claim> GetDefaultClaims(string email)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Sid, string.Empty)
            };
        return claims;
    }
}
