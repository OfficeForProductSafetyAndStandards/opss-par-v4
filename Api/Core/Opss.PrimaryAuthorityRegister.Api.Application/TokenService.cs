using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authentication;
using Opss.PrimaryAuthorityRegister.Api.Application.Settings;
using Opss.PrimaryAuthorityRegister.Authentication;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using Opss.PrimaryAuthorityRegister.Web.Application.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace Opss.PrimaryAuthorityRegister.Api.Application;

public class TokenService : ITokenService
{
    private readonly JwtAuthConfig _jwtAuthConfig;
    private readonly OneLoginAuthConfig _oneLoginAuthConfig;
    private readonly IOneLoginService _oneLoginService;

    public TokenService(IOptions<OneLoginAuthConfig> oneLoginAuthConfig, IOptions<JwtAuthConfig> jwtAuthConfig, IOneLoginService oneLoginService)
    {
        ArgumentNullException.ThrowIfNull(jwtAuthConfig);
        ArgumentNullException.ThrowIfNull(oneLoginAuthConfig);

        _jwtAuthConfig = jwtAuthConfig.Value;
        _oneLoginAuthConfig = oneLoginAuthConfig.Value;
        _oneLoginService = oneLoginService;
    }

    public string GenerateJwtToken(string email)
    {
        var key = Encoding.UTF8.GetBytes(_jwtAuthConfig.SecurityKey);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        IEnumerable<Claim> claims;
        claims = GetDefaultClaims(email);

        var token = new JwtSecurityToken(
            issuer: _jwtAuthConfig.Issuer,
            audience: _jwtAuthConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtAuthConfig.MinutesUntilExpiration),
            signingCredentials: credentials
        );

        Dictionary<string, object> claimsDict = claims
           .GroupBy(c => c.Type) // Group claims by their type
           .ToDictionary(
               g => g.Key,
               g => g.Count() > 1 ? g.Select(c => c.Value).ToArray() : (object)g.First().Value
           );

        var securityTokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer = _jwtAuthConfig.Issuer,
            Audience = _jwtAuthConfig.Audience,
            Claims = claimsDict,
            Expires = DateTime.UtcNow.AddMinutes(_jwtAuthConfig.MinutesUntilExpiration),
            SigningCredentials = credentials
        };

        return new JwtSecurityTokenHandler().CreateEncodedJwt(securityTokenDescriptor);

        //return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task ValidateTokenAsync(string idToken, CancellationToken cancellationToken)
    {
        var response = await _oneLoginService.GetSigningKeys();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response.StatusCode, response.Problem.Detail);
        }

        var keys = (response.Result?.Keys) ?? throw new AuthenticationException("GOV.UK One Login returned an empty key set");

        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = $"{_oneLoginAuthConfig.Authority}/",
            ValidAudience = _oneLoginAuthConfig.ClientId,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) => keys,
            ClockSkew = TimeSpan.FromSeconds(_oneLoginAuthConfig.ClockSkewSeconds)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.ValidateToken(idToken, tokenValidationParameters, out SecurityToken validatedToken);
    }

    private IEnumerable<Claim> GetDefaultClaims(string email)
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
