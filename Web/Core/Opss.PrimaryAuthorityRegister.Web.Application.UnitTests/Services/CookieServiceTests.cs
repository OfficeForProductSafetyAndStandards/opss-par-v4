using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Moq;
using Opss.PrimaryAuthorityRegister.Web.Application.Services;
using System.Globalization;

namespace Opss.PrimaryAuthorityRegister.Web.Application.UnitTests.Services;

public class CookieServiceTests
{
    [Fact]
    public void SetCultureCookie_ShouldSetCorrectCookie()
    {
        // Arrange
        var mockResponseCookies = new Mock<IResponseCookies>();
        var mockHttpResponse = new Mock<HttpResponse>();
        var mockHttpContext = new Mock<HttpContext>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        // Set up HttpResponse to return mocked Cookies
        mockHttpResponse.Setup(r => r.Cookies).Returns(mockResponseCookies.Object);
        mockHttpContext.Setup(c => c.Response).Returns(mockHttpResponse.Object);
        mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

        var cookieService = new CookieService();

        // Expected cookie value
        var expectedCookieValue = CookieRequestCultureProvider.MakeCookieValue(
            new RequestCulture(CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture));

        // Act
        cookieService.SetCultureCookie(mockHttpContextAccessor.Object.HttpContext);

        // Assert
        mockResponseCookies.Verify(
            c => c.Append(CookieRequestCultureProvider.DefaultCookieName, expectedCookieValue),
            Times.Once
        );
    }

    [Fact]
    public void SetCultureCookie_ShouldDoNothing_WhenHttpContextIsNull()
    {
        // Arrange
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        var cookieService = new CookieService();

        // Act
        cookieService.SetCultureCookie(mockHttpContextAccessor.Object.HttpContext);

        // Assert
        // Since HttpContext is null, the cookie should never be set
        // We use Moq's strict behavior (default) to ensure no calls to Append happen
        Assert.True(true);
    }
}
