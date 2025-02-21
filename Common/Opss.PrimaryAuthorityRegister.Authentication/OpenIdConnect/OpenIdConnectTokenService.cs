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

namespace Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;

public class OpenIdConnectTokenService : ITokenService
{
    private readonly JwtAuthConfig _jwtAuthConfig;
    private readonly OpenIdConnectAuthConfigurations _oidcAuthConfig;
    private readonly IJwtHandler _tokenHandler;
    private readonly IAuthenticatedUserService _authUserService;

    public OpenIdConnectTokenService(
        IOptions<OpenIdConnectAuthConfigurations> oneLoginAuthConfig,
        IOptions<JwtAuthConfig> jwtAuthConfig,
        IJwtHandler tokenHandler,
        IAuthenticatedUserService authUserService)
    {
        ArgumentNullException.ThrowIfNull(jwtAuthConfig);
        ArgumentNullException.ThrowIfNull(oneLoginAuthConfig);

        _jwtAuthConfig = jwtAuthConfig.Value;
        _oidcAuthConfig = oneLoginAuthConfig.Value;
        _tokenHandler = tokenHandler;
        _authUserService = authUserService;
    }

    public string GenerateJwt(string email)
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

    public async Task ValidateTokenAsync(string providerKey, string idToken, CancellationToken cancellationToken)
    {
        var response = await _authUserService.GetSigningKeys(providerKey).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response.StatusCode, response.Problem?.Detail);
        }

        var keys = (response.Result?.Keys) ?? throw new AuthenticationException("An empty key set was returned");

        var config = _oidcAuthConfig.Providers[providerKey];

        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = config.IssuerUri.ToString(),
            ValidAudience = config.ClientId,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) => keys,
            ClockSkew = TimeSpan.FromSeconds(config.ClockSkewSeconds)
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
