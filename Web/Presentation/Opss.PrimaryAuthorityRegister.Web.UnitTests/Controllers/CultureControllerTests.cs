using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Web.Controllers;
using System;

namespace Opss.PrimaryAuthorityRegister.Web.UnitTests.Controllers;

public class CultureControllerTests
{
    [Fact]
    public void Set_Should_Set_Cookie_When_Culture_Is_Provided()
    {
        // Arrange
        using var controller = new CultureController();
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        string culture = "fr-FR";
        Uri redirectUri = new Uri("/home", UriKind.Relative);

        // Act
        var result = controller.Set(culture, redirectUri) as LocalRedirectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(redirectUri.ToString(), result.Url);
    }

    [Fact]
    public void Set_Should_Not_Set_Cookie_When_Culture_Is_Null()
    {
        // Arrange
        using var controller = new CultureController();
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        string? culture = null;
        Uri redirectUri = new Uri("/home", UriKind.Relative);

        // Act
#pragma warning disable CS8604 // Possible null reference argument.
        var result = controller.Set(culture, redirectUri) as LocalRedirectResult;
#pragma warning restore CS8604 // Possible null reference argument.

        // Assert
        Assert.NotNull(result);
        Assert.Equal(redirectUri.ToString(), result.Url);
    }
}
