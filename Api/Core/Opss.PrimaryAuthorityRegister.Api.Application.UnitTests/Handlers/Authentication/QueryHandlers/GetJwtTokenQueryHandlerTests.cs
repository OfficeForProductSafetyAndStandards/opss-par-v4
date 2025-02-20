using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Authentication.QueryHandlers;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using Opss.PrimaryAuthorityRegister.Http.Problem;
using System.Net;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Handlers.Authentication.QueryHandlers;

public class GetJwtTokenQueryHandlerTests
{
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IAuthenticatedUserService> _oneLoginServiceMock;
    private readonly GetJwtTokenQueryHandler _handler;

    public GetJwtTokenQueryHandlerTests()
    {
        _tokenServiceMock = new Mock<ITokenService>();
        _oneLoginServiceMock = new Mock<IAuthenticatedUserService>();
        _handler = new GetJwtTokenQueryHandler(_tokenServiceMock.Object, _oneLoginServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenValidRequest()
    {
        // Arrange
        var query = new GetJwtTokenQuery("Provider", "validIdToken", "validAccessToken");
        using var okMessage = new HttpResponseMessage(HttpStatusCode.OK);

        var expectedToken = "jwtToken";
        var userInfoResponse = new HttpObjectResponse<AuthenticatedUserInfoDto>(
            okMessage,
            new AuthenticatedUserInfoDto("sub", DateTime.Now.ToString()) { Email = "test@example.com" });

        _tokenServiceMock.Setup(ts => ts.ValidateTokenAsync(query.ProviderKey, query.IdToken, It.IsAny<CancellationToken>()))
                          .Returns(Task.CompletedTask);
        _oneLoginServiceMock.Setup(os => os.GetUserInfo(query.AccessToken))
                            .ReturnsAsync(userInfoResponse);
        _tokenServiceMock.Setup(ts => ts.GenerateJwtToken("test@example.com"))
                          .Returns(expectedToken);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(expectedToken, result);
    }


    [Fact]
    public async Task Handle_ShouldThrowException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
    }


    [Fact]
    public async Task Handle_ShouldThrowHttpResponseException_WhenUserServiceFails()
    {
        // Arrange
        var query = new GetJwtTokenQuery("Provider", "validIdToken", "validAccessToken");
        using var badRequestMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

        var errorResponse = new HttpObjectResponse<AuthenticatedUserInfoDto>(
            badRequestMessage, 
            null,
            new ProblemDetails(HttpStatusCode.BadRequest, new Exception("Invalid token"))
        );

        _tokenServiceMock.Setup(ts => ts.ValidateTokenAsync(query.ProviderKey, query.IdToken, It.IsAny<CancellationToken>()))
                          .Returns(Task.CompletedTask);
        _oneLoginServiceMock.Setup(os => os.GetUserInfo(query.AccessToken))
                            .ReturnsAsync(errorResponse);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() => _handler.Handle(query, CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        Assert.Equal("Invalid token", exception.Message);
    }
}
