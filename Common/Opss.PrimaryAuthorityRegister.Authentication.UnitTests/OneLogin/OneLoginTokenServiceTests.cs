using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Authentication;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.OneLogin;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Authentication.TokenHandler;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using Opss.PrimaryAuthorityRegister.Http.Problem;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.OneLogin;

public class OneLoginTokenServiceTests
{
    private readonly Mock<IAuthenticatedUserService> _mockAuthenticatedUserService;
    private readonly Mock<IJwtTokenHandler> _mockJwtTokenHandler;
    private readonly IOptions<JwtAuthConfig> _jwtAuthConfig;
    private readonly IOptions<OpenIdConnectAuthConfig> _oneLoginAuthConfig;
    private readonly OneLoginTokenService _tokenService;

    public OneLoginTokenServiceTests()
    {
        _mockAuthenticatedUserService = new Mock<IAuthenticatedUserService>();
        _mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
        _jwtAuthConfig = Options.Create(new JwtAuthConfig
        {
            SecurityKey = "SuperSecretKey1234567890SuperSecretKey1234567890",
            Issuer = "https://issuer.example.com",
            Audience = "https://audience.example.com",
            MinutesUntilExpiration = 60
        });
        _oneLoginAuthConfig = Options.Create(new OpenIdConnectAuthConfig
        {
            Authority = "https://auth.example.com",
            ClientId = "client-id",
            ClockSkewSeconds = 300,
            CookieMaxAge = 30,
            PostLogoutRedirectUri = new Uri("https://localhost"), 
            RsaPrivateKey = "PrivateKey"
        });

        _tokenService = new OneLoginTokenService(
            _oneLoginAuthConfig, 
            _jwtAuthConfig,
            _mockJwtTokenHandler.Object, 
            _mockAuthenticatedUserService.Object);
    }

    [Fact]
    public void GenerateJwtToken_ShouldReturnValidJwt()
    {
        // Arrange
        string email = "test@example.com";

        // Act
        string token = _tokenService.GenerateJwtToken(email);

        // Assert
        Assert.NotNull(token);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        Assert.Equal("https://issuer.example.com", jwt.Issuer);
        Assert.Equal("https://audience.example.com", jwt.Audiences.First());
        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
    }

    [Fact]
    public async Task ValidateTokenAsync_InvalidSigningKeys_ShouldThrowAuthenticationException()
    {
        // Arrange
        string idToken = "invalid-token";

        using var okCode = new HttpResponseMessage(HttpStatusCode.OK);
        var response = new HttpObjectResponse<JsonWebKeySet>(okCode, null, null);
        _mockAuthenticatedUserService.Setup(s => s.GetSigningKeys()).ReturnsAsync(response);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(async () =>
            await _tokenService.ValidateTokenAsync(idToken, CancellationToken.None).ConfigureAwait(true))
            .ConfigureAwait(true);
    }

    [Fact]
    public async Task ValidateTokenAsync_HttpErrorResponse_ShouldThrowHttpResponseException()
    {
        // Arrange
        string idToken = "invalid-token";

        using var errorCode = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var response = new HttpObjectResponse<JsonWebKeySet>(
                    errorCode,
                    null,
                    new ProblemDetails(
                        HttpStatusCode.BadRequest,
                        new Exception("Error"), true
                    )
                );
        _mockAuthenticatedUserService.Setup(s => s.GetSigningKeys()).ReturnsAsync(response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(async () =>
            await _tokenService.ValidateTokenAsync(idToken, CancellationToken.None).ConfigureAwait(true))
            .ConfigureAwait(true);
        Assert.Equal(HttpStatusCode.BadRequest, exception.Response.StatusCode);
    }


    [Fact]
    public async Task ValidateTokenAsync_ValidToken_ShouldNotThrowException()
    {
        // Arrange
        var jsonResponse = "{ \"keys\": [{ \"kty\": \"RSA\", \"use\": \"sig\" }] }";
        var expectedKeys = new JsonWebKeySet(jsonResponse);

        using var okCode = new HttpResponseMessage(HttpStatusCode.OK);
        string idToken = "valid-token";
        var response = new HttpObjectResponse<JsonWebKeySet>(okCode, expectedKeys, null);
        _mockAuthenticatedUserService.Setup(s => s.GetSigningKeys()).ReturnsAsync(response);

        _mockJwtTokenHandler.Setup(h => h.ValidateToken(idToken, It.IsAny<TokenValidationParameters>(), out It.Ref<SecurityToken>.IsAny)).Verifiable();

        // Act & Assert
        await _tokenService.ValidateTokenAsync(idToken, CancellationToken.None);
        _mockJwtTokenHandler.Verify(h => h.ValidateToken(idToken, It.IsAny<TokenValidationParameters>(), out It.Ref<SecurityToken>.IsAny), Times.Once);
    }
}

