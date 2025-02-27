using Microsoft.AspNetCore.Http;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.ServiceProviders;

public class UserClaimsServiceProviderTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IUserClaimsService> _userClaimsServiceMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;

    public UserClaimsServiceProviderTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _userClaimsServiceMock = new Mock<IUserClaimsService>();
        _serviceProviderMock = new Mock<IServiceProvider>();

        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IHttpContextAccessor)))
                            .Returns(_httpContextAccessorMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IUserClaimsService)))
                            .Returns(_userClaimsServiceMock.Object);
    }

    [Fact]
    public void BuildClaims_ShouldThrowArgumentNullException_WhenUserIsNull()
    {
        // Arrange
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns((HttpContext)null);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => UserClaimsServiceProvider.BuildClaims(_serviceProviderMock.Object));
    }

    [Fact]
    public void BuildClaims_ShouldReturnClaimsPrincipal_WithUserClaims()
    {
        // Arrange
        var email = "test@example.com";
        var userClaims = new[] { new Claim("CustomClaim", "Value") };
        var claims = new[] { new Claim(ClaimTypes.Email, email) };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);
        _userClaimsServiceMock.Setup(s => s.GetUserClaims(email)).Returns(userClaims);

        // Act
        var result = UserClaimsServiceProvider.BuildClaims(_serviceProviderMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result.Claims, c => c.Type == "CustomClaim" && c.Value == "Value");
        Assert.Contains(result.Claims, c => c.Type == ClaimTypes.Email && c.Value == email);
    }
}
