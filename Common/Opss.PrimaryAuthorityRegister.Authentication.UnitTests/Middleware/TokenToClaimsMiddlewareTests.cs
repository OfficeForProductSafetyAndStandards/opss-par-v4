using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.Jwt;
using Opss.PrimaryAuthorityRegister.Authentication.Middleware;


namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.Middleware;

public class TokenToClaimsMiddlewareTests
{
    [Fact]
    public async Task Invoke_WithValidToken_SetsHttpContextUser()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Bearer valid_token";

        var jwtServiceMock = new Mock<IJwtService>();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "test_user") }));
        var jwtToken = new JwtSecurityToken();

        jwtServiceMock
            .Setup(s => s.ValidateToken("valid_token", out It.Ref<SecurityToken>.IsAny))
            .Callback((string token, out SecurityToken validatedToken) =>
            {
                validatedToken = jwtToken;
            })
            .Returns(claimsPrincipal);

        var middleware = new TokenToClaimsMiddleware(next: (innerContext) => Task.CompletedTask);

        // Act
        await middleware.Invoke(context, jwtServiceMock.Object);

        // Assert
        Assert.NotNull(context.User);
        Assert.Equal("test_user", context.User.Identity.Name);
    }

    [Fact]
    public async Task Invoke_WithInvalidToken_DoesNotSetHttpContextUser()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Bearer invalid_token";

        var jwtServiceMock = new Mock<IJwtService>();
        jwtServiceMock
            .Setup(s => s.ValidateToken("invalid_token", out It.Ref<SecurityToken>.IsAny))
            .Returns((ClaimsPrincipal)null);

        var middleware = new TokenToClaimsMiddleware(next: (innerContext) => Task.CompletedTask);

        // Act
        await middleware.Invoke(context, jwtServiceMock.Object);

        // Assert
        Assert.False(context.User.Identity.IsAuthenticated);
    }

    [Fact]
    public async Task Invoke_WithoutAuthorizationHeader_DoesNotModifyHttpContextUser()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var jwtServiceMock = new Mock<IJwtService>();
        var middleware = new TokenToClaimsMiddleware(next: (innerContext) => Task.CompletedTask);

        // Act
        await middleware.Invoke(context, jwtServiceMock.Object);

        // Assert
        Assert.False(context.User.Identity.IsAuthenticated);
    }
}
