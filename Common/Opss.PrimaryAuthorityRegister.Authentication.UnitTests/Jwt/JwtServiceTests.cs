using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using Opss.PrimaryAuthorityRegister.Authentication.Jwt;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.Jwt;

public class JwtServiceTests
{
    private readonly Mock<IUserRoleService> _mockUserRoleService;
    private readonly JwtService _jwtService;
    private readonly JwtAuthConfig _jwtAuthConfig;

    public JwtServiceTests()
    {
        _jwtAuthConfig = new JwtAuthConfig
        {
            SecurityKey = "ThisIsASecretKeyForTestingPurposes1234",
            IssuerUri = new Uri("https://test-issuer.com"),
            AudienceUri = new Uri("https://test-audience.com"),
            MinutesUntilExpiration = 60,
            ClockSkewSeconds = 30
        };

        var options = Options.Create(_jwtAuthConfig);
        _mockUserRoleService = new Mock<IUserRoleService>();
        _jwtService = new JwtService(options, _mockUserRoleService.Object);
    }

    [Fact]
    public void GenerateJwt_ShouldReturnValidToken()
    {
        // Arrange
        var email = "user@example.com";
        var roles = new AuthenticatedUserRole[] { new AuthenticatedUserRole("Admin"), new AuthenticatedUserRole("User") };
        _mockUserRoleService.Setup(x => x.GetUserWithRolesByEmailAddress(email)).Returns(
            new AuthenticatedUserIdentity(Guid.NewGuid(), email,
            roles));

        // Act
        var token = _jwtService.GenerateJwt(email);

        // Assert
        Assert.NotNull(token);
        Assert.False(string.IsNullOrEmpty(token));

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.Equal(_jwtAuthConfig.IssuerUri.ToString(), jwtToken.Issuer);
        Assert.Equal(_jwtAuthConfig.AudienceUri.ToString(), jwtToken.Audiences.FirstOrDefault());
        Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
    }

    [Fact]
    public void GenerateJwt_ShouldReturnValidToken_WhenIdentityNotFound()
    {
        // Arrange
        var email = "user@example.com";
        _mockUserRoleService.Setup(x => x.GetUserWithRolesByEmailAddress(email)).Returns(
            () => null);

        // Act
        var token = _jwtService.GenerateJwt(email);

        // Assert
        Assert.NotNull(token);
        Assert.False(string.IsNullOrEmpty(token));

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.Equal(_jwtAuthConfig.IssuerUri.ToString(), jwtToken.Issuer);
        Assert.Equal(_jwtAuthConfig.AudienceUri.ToString(), jwtToken.Audiences.FirstOrDefault());
        Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        Assert.DoesNotContain(jwtToken.Claims, c => c.Type == ClaimTypes.Role);
    }

    [Fact]
    public void GenerateJwt_ShouldReturnValidToken_WhenNoEmailProvided()
    {
        // Arrange
        string? email = null;
        _mockUserRoleService.Setup(x => x.GetUserWithRolesByEmailAddress(email)).Returns(
            () => null);

        // Act
        var token = _jwtService.GenerateJwt(email);

        // Assert
        Assert.NotNull(token);
        Assert.False(string.IsNullOrEmpty(token));

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.Equal(_jwtAuthConfig.IssuerUri.ToString(), jwtToken.Issuer);
        Assert.Equal(_jwtAuthConfig.AudienceUri.ToString(), jwtToken.Audiences.FirstOrDefault());
        Assert.DoesNotContain(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Email);
        Assert.DoesNotContain(jwtToken.Claims, c => c.Type == ClaimTypes.Role);
    }

    [Fact]
    public void ValidateToken_ShouldReturnPrincipal_WhenTokenIsValid()
    {
        // Arrange
        var email = "user@example.com";
        var roles = new AuthenticatedUserRole[] { new AuthenticatedUserRole("Admin"), new AuthenticatedUserRole("User") };
        _mockUserRoleService.Setup(x => x.GetUserWithRolesByEmailAddress(email)).Returns(
            new AuthenticatedUserIdentity(Guid.NewGuid(), email,
            roles));

        var token = _jwtService.GenerateJwt(email);

        // Act
        var principal = _jwtService.ValidateToken(token, out var validatedToken);

        // Assert
        Assert.NotNull(principal);
        Assert.NotNull(validatedToken);
        Assert.Contains(principal.Claims, c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" && c.Value == email);
    }

    [Fact]
    public void ValidateToken_ShouldReturnNull_WhenTokenIsInvalid()
    {
        // Arrange
        var invalidToken = "invalid.token.string";

        // Act
        SecurityToken validatedToken;

        // Assert
        Assert.Throws<ArgumentException>(() => _jwtService.ValidateToken(invalidToken, out validatedToken));
    }
}
