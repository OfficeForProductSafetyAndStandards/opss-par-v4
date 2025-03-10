using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Opss.PrimaryAuthorityRegister.Web.Controllers;
using Opss.PrimaryAuthorityRegister.Cqrs.Services;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Profile.Queries;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Opss.PrimaryAuthorityRegister.Web.UnitTests.Controllers;

public class OidcControllerTests
{
    private readonly OidcController _controller;
    private readonly Mock<HttpRequest> _httpRequestMock;
    private readonly Mock<ICqrsService> _cqrsServiceMock;

    public OidcControllerTests()
    {
        Mock<HttpContext> httpContextMock = new();
        _httpRequestMock = new Mock<HttpRequest>();
        _cqrsServiceMock = new Mock<ICqrsService>();

        httpContextMock.Setup(ctx => ctx.Request).Returns(_httpRequestMock.Object);

        _controller = new OidcController(_cqrsServiceMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object }
        };
    }

    [Fact]
    public void WhenLogin_WithNullReturnUrl_ShouldRedirectToRoot()
    {
        // Arrange
        _httpRequestMock.Setup(r => r.PathBase).Returns(new PathString("/"));

        // Act
        var result = _controller.Login(null) as ChallengeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("oidc-onelogin", result.AuthenticationSchemes[0]);
        Assert.Equal("/", result.Properties!.RedirectUri);
    }

    [Fact]
    public void WhenLogin_WithReturnUrl_ShouldRedirectToProvidedUrl()
    {
        // Arrange
        var returnUrl = new Uri("/dashboard", UriKind.Relative);

        // Act
        var result = _controller.Login(returnUrl) as ChallengeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("oidc-onelogin", result.AuthenticationSchemes[0]);
        Assert.Equal("/dashboard", result.Properties!.RedirectUri);
    }

    [Fact]
    public void WhenLogout_WithNullReturnUrl_ShouldRedirectToRoot()
    {
        // Arrange
        _httpRequestMock.Setup(r => r.PathBase).Returns(new PathString("/"));

        // Act
        var result = _controller.Logout(null) as SignOutResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains(CookieAuthenticationDefaults.AuthenticationScheme, result.AuthenticationSchemes);
        Assert.Contains("oidc-onelogin", result.AuthenticationSchemes);
        Assert.Equal("/", result.Properties!.RedirectUri);
    }

    [Fact]
    public void WhenLogout_WithReturnUrl_ShouldRedirectToProvidedUrl()
    {
        // Arrange
        var returnUrl = new Uri("/home", UriKind.Relative);

        // Act
        var result = _controller.Logout(returnUrl) as SignOutResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains(CookieAuthenticationDefaults.AuthenticationScheme, result.AuthenticationSchemes);
        Assert.Contains("oidc-onelogin", result.AuthenticationSchemes);
        Assert.Equal("/home", result.Properties!.RedirectUri);
    }

    [Fact]
    public async Task  WhenCallingAfterLogin_AndAgreedTandCs_ThenRedirectsToDashboard()
    {
        // Arrange
        using var msg =new HttpResponseMessage();
        _cqrsServiceMock
            .Setup(c => c.GetAsync<GetMyProfileQuery, MyProfileDto>(It.IsAny<GetMyProfileQuery>()))
            .ReturnsAsync(new HttpObjectResponse<MyProfileDto>(msg, new MyProfileDto(true)));

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
        using var msg =new HttpResponseMessage();
        _cqrsServiceMock
            .Setup(c => c.GetAsync<GetMyProfileQuery, MyProfileDto>(It.IsAny<GetMyProfileQuery>()))
            .ReturnsAsync(new HttpObjectResponse<MyProfileDto>(msg, new MyProfileDto(false)));

        // Act
        var result = await _controller.AfterLogin() as RedirectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("/terms-conditions", result.Url);
    }

    [Fact]
    public async Task WhenCallingAfterLogin_AndProfileNotFound_ThenRedirectsToTandCs()
    {
        // Arrange
        using var msg =new HttpResponseMessage();
        _cqrsServiceMock
            .Setup(c => c.GetAsync<GetMyProfileQuery, MyProfileDto>(It.IsAny<GetMyProfileQuery>()))
            .ReturnsAsync(new HttpObjectResponse<MyProfileDto>(msg, null));

        // Act
        var result = await _controller.AfterLogin() as RedirectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("/terms-conditions", result.Url);
    }
}

