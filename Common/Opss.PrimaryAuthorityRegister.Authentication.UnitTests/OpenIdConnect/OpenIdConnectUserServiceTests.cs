using Microsoft.Extensions.Options;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Http.Services;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using System.Net;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Problem;
using Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.OneLogin;

public class OpenIdConnectUserServiceTests
{
    private readonly Mock<IHttpService> _httpServiceMock;
    private readonly OpenIdConnectUserService _oneLoginService;

    public OpenIdConnectUserServiceTests()
    {
        var optionsMock = new Mock<IOptions<OpenIdConnectAuthConfigurations>>();
        optionsMock.Setup(o => o.Value).Returns(AuthenticationTestHelpers.ProviderAuthConfigurations);

        _httpServiceMock = new Mock<IHttpService>(MockBehavior.Strict);

        _oneLoginService = new OpenIdConnectUserService(optionsMock.Object, _httpServiceMock.Object);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenOptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new OpenIdConnectUserService(null, _httpServiceMock.Object));
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
        var result = await _oneLoginService.GetSigningKeys(AuthenticationTestHelpers.AuthProviderKey).ConfigureAwait(true);

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
        var result = await _oneLoginService.GetSigningKeys(AuthenticationTestHelpers.AuthProviderKey).ConfigureAwait(true);

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
        var result = await _oneLoginService.GetUserInfo(AuthenticationTestHelpers.AuthProviderKey, accessToken).ConfigureAwait(true);

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
        var result = await _oneLoginService.GetUserInfo(AuthenticationTestHelpers.AuthProviderKey, accessToken).ConfigureAwait(true);


        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccessStatusCode);
        Assert.Null(result.Result);
    }
}
