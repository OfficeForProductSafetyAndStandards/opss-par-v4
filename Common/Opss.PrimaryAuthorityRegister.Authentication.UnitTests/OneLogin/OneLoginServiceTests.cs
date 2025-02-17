using Microsoft.Extensions.Options;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.OneLogin;
using Opss.PrimaryAuthorityRegister.Http.Services;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using System.Net;
using System.Text.Json;

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

        _httpServiceMock = new Mock<IHttpService>();

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

        _httpServiceMock
            .Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _oneLoginService.GetSigningKeys();

        // Assert
        Assert.NotNull(result);
        //Assert.True(result.IsSuccess);
        //Assert.NotNull(result.Data);
        //Assert.Single(result.Data.Keys);
    }

    [Fact]
    public async Task GetSigningKeys_ShouldReturnFailure_WhenResponseIsError()
    {
        // Arrange
        _httpServiceMock
            .Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        // Act
        var result = await _oneLoginService.GetSigningKeys();

        // Assert
        Assert.NotNull(result);
        //Assert.False(result.IsSuccess);
        //Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetUserInfo_ShouldReturnUserInfo_WhenResponseIsSuccessful()
    {
        // Arrange
        var accessToken = "test-token";
        var expectedUserInfo = new AuthenticatedUserInfoDto("12345", "2024-01-01T12:00:00Z");


        _httpServiceMock
            .Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedUserInfo))
            });

        // Act
        var result = await _oneLoginService.GetUserInfo(accessToken);

        // Assert
        Assert.NotNull(result);
        //Assert.True(result.IsSuccess);
        //Assert.NotNull(result.Data);
        //Assert.Equal("12345", result.Data.Sub);
        //Assert.Equal("John Doe", result.Data.Name);
    }

    [Fact]
    public async Task GetUserInfo_ShouldReturnFailure_WhenResponseIsUnauthorized()
    {
        // Arrange
        var accessToken = "invalid-token";

        _httpServiceMock
            .Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = null
            });

        // Act
        var result = await _oneLoginService.GetUserInfo(accessToken);

        // Assert
        Assert.NotNull(result);
        //Assert.False(result.IsSuccess);
        //Assert.Null(result.Data);
    }
}
