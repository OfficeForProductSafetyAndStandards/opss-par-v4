using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Opss.PrimaryAuthorityRegister.Web.Controllers;
using Opss.PrimaryAuthorityRegister.Cqrs.Services;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Profile.Queries;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using System.Net.Http;
using System.Threading.Tasks;

namespace Opss.PrimaryAuthorityRegister.Web.UnitTests.Controllers;

public class OidcControllerTests
{
    private readonly OidcController _controller;
    private readonly Mock<HttpContext> _httpContextMock;
    private readonly Mock<HttpRequest> _httpRequestMock;
    private readonly Mock<ICqrsService> _cqrsServiceMock;

    public OidcControllerTests()
    {
        _httpContextMock = new Mock<HttpContext>();
        _httpRequestMock = new Mock<HttpRequest>();
        _cqrsServiceMock = new Mock<ICqrsService>();

        _httpContextMock.Setup(ctx => ctx.Request).Returns(_httpRequestMock.Object);

        _controller = new OidcController(_cqrsServiceMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = _httpContextMock.Object }
        };
    }

    [Fact]
    public void Login_WithNullReturnUrl_ShouldRedirectToRoot()
    {
        // Arrange
        _httpRequestMock.Setup(r => r.PathBase).Returns(new PathString("/"));

        // Act
        var result = _controller.Login(null) as ChallengeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("oidc-onelogin", result.AuthenticationSchemes[0]);
        Assert.Equal("/", result.Properties.RedirectUri);
    }

    [Fact]
    public void Login_WithReturnUrl_ShouldRedirectToProvidedUrl()
    {
        // Arrange
        var returnUrl = new Uri("/dashboard", UriKind.Relative);

        // Act
        var result = _controller.Login(returnUrl) as ChallengeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("oidc-onelogin", result.AuthenticationSchemes[0]);
        Assert.Equal("/dashboard", result.Properties.RedirectUri);
    }

    [Fact]
    public void Logout_WithNullReturnUrl_ShouldRedirectToRoot()
    {
        // Arrange
        _httpRequestMock.Setup(r => r.PathBase).Returns(new PathString("/"));

        // Act
        var result = _controller.Logout(null) as SignOutResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains(CookieAuthenticationDefaults.AuthenticationScheme, result.AuthenticationSchemes);
        Assert.Contains("oidc-onelogin", result.AuthenticationSchemes);
        Assert.Equal("/", result.Properties.RedirectUri);
    }

    [Fact]
    public void Logout_WithReturnUrl_ShouldRedirectToProvidedUrl()
    {
        // Arrange
        var returnUrl = new Uri("/home", UriKind.Relative);

        // Act
        var result = _controller.Logout(returnUrl) as SignOutResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains(CookieAuthenticationDefaults.AuthenticationScheme, result.AuthenticationSchemes);
        Assert.Contains("oidc-onelogin", result.AuthenticationSchemes);
        Assert.Equal("/home", result.Properties.RedirectUri);
    }

    [Fact]
    public async Task  WhenCallingAfterLogin_AndAgreedTandCs_ThenRedirectsToDashboard()
    {
        // Arrange
        _cqrsServiceMock
            .Setup(c => c.GetAsync<GetMyProfileQuery, MyProfileDto>(It.IsAny<GetMyProfileQuery>()))
            .ReturnsAsync(new HttpObjectResponse<MyProfileDto>(new HttpResponseMessage(), new MyProfileDto(true)));

        // Act
        var result = await _controller.AfterLogin() as RedirectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("/authority", result.Url);
    }

    [Fact]
    public async Task WhenCallingAfterLogin_AndNotAgreedTandCs_ThenRedirectsToTandCs()
    {
        // Arrange
        _cqrsServiceMock
            .Setup(c => c.GetAsync<GetMyProfileQuery, MyProfileDto>(It.IsAny<GetMyProfileQuery>()))
            .ReturnsAsync(new HttpObjectResponse<MyProfileDto>(new HttpResponseMessage(), new MyProfileDto(false)));

        // Act
        var result = await _controller.AfterLogin() as RedirectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("/terms-conditions", result.Url);
    }
}

