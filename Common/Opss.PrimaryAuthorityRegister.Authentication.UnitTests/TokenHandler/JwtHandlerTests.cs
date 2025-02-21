using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.TokenHandler;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.TokenHandler;

public class JwtHandlerTests
{
    private readonly JwtHandler _jwtHandler;

    public JwtHandlerTests()
    {
        _jwtHandler = new JwtHandler();
    }

    [Fact]
    public void ValidateToken_ValidToken_ReturnsClaimsPrincipal()
    {
        // Arrange
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Convert.FromBase64String(AuthenticationTestHelpers.JwtConfig.SecurityKey);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = credentials
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        // Act
        var principal = _jwtHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

        // Assert
        Assert.NotNull(principal);
        Assert.NotNull(validatedToken);
        Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Name && c.Value == "TestUser");
    }
}
