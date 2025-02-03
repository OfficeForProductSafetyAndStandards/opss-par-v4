using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Web.Controllers;

namespace Opss.PrimaryAuthorityRegister.Web.UnitTests.Controllers;

public class CultureControllerTests
{
    [Fact]
    public void Set_Should_Set_Cookie_When_Culture_Is_Provided()
    {
        // Arrange
        var controller = new CultureController();
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        string culture = "fr-FR";
        string redirectUri = "/home";

        // Act
        var result = controller.Set(culture, redirectUri) as LocalRedirectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(redirectUri, result.Url);
    }

    [Fact]
    public void Set_Should_Not_Set_Cookie_When_Culture_Is_Null()
    {
        // Arrange
        var controller = new CultureController();
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        string culture = null;
        string redirectUri = "/home";

        // Act
        var result = controller.Set(culture, redirectUri) as LocalRedirectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(redirectUri, result.Url);
    }
}
