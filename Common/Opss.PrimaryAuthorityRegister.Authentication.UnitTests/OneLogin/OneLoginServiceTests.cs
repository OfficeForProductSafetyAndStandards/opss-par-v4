using Microsoft.Extensions.Options;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.OneLogin;
using Opss.PrimaryAuthorityRegister.Http.Services;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using System.Net;
using System.Text.Json;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Problem;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.OneLogin;

public class OneLoginServiceTests
{
    private readonly Mock<IHttpService> _httpServiceMock;
    private readonly OneLoginService _oneLoginService;
    private readonly OpenIdConnectAuthConfig _authConfig;

    public OneLoginServiceTests()
    {
        _authConfig = new OpenIdConnectAuthConfig
        {
            Authority = "https://example.com",
            ClientId = "client-id",
            CookieMaxAge = 60,
            ClockSkewSeconds = 300,
            PostLogoutRedirectUri = new Uri("https://localhost/"),
            RsaPrivateKey = "RSAKey"
        };

        var optionsMock = new Mock<IOptions<OpenIdConnectAuthConfig>>();
        optionsMock.Setup(o => o.Value).Returns(_authConfig);

        _httpServiceMock = new Mock<IHttpService>(MockBehavior.Strict);

        _oneLoginService = new OneLoginService(optionsMock.Object, _httpServiceMock.Object);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenOptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new OneLoginService(null, _httpServiceMock.Object));
    }

    [Fact]
    public async Task GetSigningKeys_ShouldReturnKeys_WhenResponseIsSuccessful()
    {
        // Arrange
        var jsonResponse = "{ \"keys\": [{ \"kty\": \"RSA\", \"use\": \"sig\" }] }";
        var expectedKeys = new JsonWebKeySet(jsonResponse);

        using var okCode = new HttpResponseMessage(HttpStatusCode.OK);
        _httpServiceMock
            .Setup(m => m.HttpSendAsync<JsonWebKeySet>(HttpMethod.Get, It.IsAny<Uri>(), null, null))
            .ReturnsAsync(new HttpObjectResponse<JsonWebKeySet>(okCode, expectedKeys, null));

        // Act
        var result = await _oneLoginService.GetSigningKeys().ConfigureAwait(true);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(result.Result);
        Assert.Single(result.Result.Keys);
    }

    [Fact]
    public async Task GetSigningKeys_ShouldReturnFailure_WhenResponseIsError()
    {
        // Arrange
        using var errorCode = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        _httpServiceMock
            .Setup(m => m.HttpSendAsync<JsonWebKeySet>(HttpMethod.Get, It.IsAny<Uri>(), null, null))
            .ReturnsAsync(
                new HttpObjectResponse<JsonWebKeySet>(
                    errorCode, 
                    null, 
                    new ProblemDetails(
                        HttpStatusCode.InternalServerError,
                        new Exception("Error"), true
                    )
                )
            );

        // Act
        var result = await _oneLoginService.GetSigningKeys().ConfigureAwait(true);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccessStatusCode);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task GetUserInfo_ShouldReturnUserInfo_WhenResponseIsSuccessful()
    {
        // Arrange
        var accessToken = "test-token";
        var expectedUserInfo = new AuthenticatedUserInfoDto("12345", "2024-01-01T12:00:00Z");

        using var okCode = new HttpResponseMessage(HttpStatusCode.OK);
        _httpServiceMock
            .Setup(m => m.HttpSendAsync<AuthenticatedUserInfoDto>(HttpMethod.Get, It.IsAny<Uri>(), null, accessToken))
            .ReturnsAsync(new HttpObjectResponse<AuthenticatedUserInfoDto>(okCode, expectedUserInfo, null));

        // Act
        var result = await _oneLoginService.GetUserInfo(accessToken).ConfigureAwait(true);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("12345", result.Result.Sub);
    }

    [Fact]
    public async Task GetUserInfo_ShouldReturnFailure_WhenResponseIsUnauthorized()
    {
        // Arrange
        var accessToken = "invalid-token";

        using var errorCode = new HttpResponseMessage(HttpStatusCode.BadRequest);
        _httpServiceMock
            .Setup(m => m.HttpSendAsync<AuthenticatedUserInfoDto>(HttpMethod.Get, It.IsAny<Uri>(), null, accessToken))
            .ReturnsAsync(
                new HttpObjectResponse<AuthenticatedUserInfoDto>(
                    errorCode,
                    null,
                    new ProblemDetails(
                        HttpStatusCode.BadRequest,
                        new Exception("Error"), true
                    )
                )
            );

        // Act
        var result = await _oneLoginService.GetUserInfo(accessToken).ConfigureAwait(true);


        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccessStatusCode);
        Assert.Null(result.Result);
    }
}
