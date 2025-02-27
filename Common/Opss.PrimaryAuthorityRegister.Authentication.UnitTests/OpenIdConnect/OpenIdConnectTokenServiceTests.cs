using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Authentication;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.Jwt;
using Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Authentication.TokenHandler;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using Opss.PrimaryAuthorityRegister.Http.Problem;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.OneLogin;

public class OpenIdConnectTokenServiceTests
{
    private readonly Mock<IAuthenticatedUserService> _mockAuthenticatedUserService;
    private readonly Mock<IJwtHandler> _mockJwtHandler;
    private readonly OpenIdConnectTokenService _tokenService;

    public OpenIdConnectTokenServiceTests()
    {
        _mockAuthenticatedUserService = new Mock<IAuthenticatedUserService>();
        _mockJwtHandler = new Mock<IJwtHandler>();

        _tokenService = new OpenIdConnectTokenService(
            Options.Create(AuthenticationTestHelpers.ProviderAuthConfigurations),
            _mockJwtHandler.Object, 
            _mockAuthenticatedUserService.Object);
    }


    [Fact]
    public async Task ValidateTokenAsync_InvalidSigningKeys_ShouldThrowAuthenticationException()
    {
        // Arrange
        string idToken = "invalid-token";

        using var okCode = new HttpResponseMessage(HttpStatusCode.OK);
        var response = new HttpObjectResponse<JsonWebKeySet>(okCode, null, null);
        _mockAuthenticatedUserService.Setup(s => s.GetSigningKeys(AuthenticationTestHelpers.AuthProviderKey)).ReturnsAsync(response);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(async () =>
            await _tokenService.ValidateTokenAsync(AuthenticationTestHelpers.AuthProviderKey, idToken, CancellationToken.None).ConfigureAwait(true))
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
        _mockAuthenticatedUserService.Setup(s => s.GetSigningKeys(AuthenticationTestHelpers.AuthProviderKey)).ReturnsAsync(response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(async () =>
            await _tokenService.ValidateTokenAsync(AuthenticationTestHelpers.AuthProviderKey, idToken, CancellationToken.None).ConfigureAwait(true))
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
        _mockAuthenticatedUserService.Setup(s => s.GetSigningKeys(AuthenticationTestHelpers.AuthProviderKey)).ReturnsAsync(response);

        _mockJwtHandler.Setup(h => h.ValidateToken(idToken, It.IsAny<TokenValidationParameters>(), out It.Ref<SecurityToken>.IsAny)).Verifiable();

        // Act & Assert
        await _tokenService.ValidateTokenAsync(AuthenticationTestHelpers.AuthProviderKey, idToken, CancellationToken.None);
        _mockJwtHandler.Verify(h => h.ValidateToken(idToken, It.IsAny<TokenValidationParameters>(), out It.Ref<SecurityToken>.IsAny), Times.Once);
    }
}

