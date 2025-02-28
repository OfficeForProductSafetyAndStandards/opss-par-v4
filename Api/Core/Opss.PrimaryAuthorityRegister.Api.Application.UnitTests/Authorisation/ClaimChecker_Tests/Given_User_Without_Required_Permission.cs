using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Fakes;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using System.Security;
using System.Security.Claims;
using Claim = System.Security.Claims.Claim;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.ClaimChecker_Tests;

public class Given_User_Without_Required_Permission
{
    [Fact]
    public void When_Invoked_Then_Throws_SecurityException()
    {
        // Arrange
        var userIdentity = new FakeClaimsPrincipal("Fred");
        var resourceClaimProvider = new FakeResourceClaimProvider();
        resourceClaimProvider.AddClaimSet(
                                          new Claim(
                                                  "http://www.example.com/Claim/Permissions/Read",
                                                  "http://www.example.com/Claim/Resource/4",
                                                  ClaimTypes.NameIdentifier));

        var claimChecker = new ClaimChecker(resourceClaimProvider);

        // Act
        var exception =
                Record.Exception(
                                 () => claimChecker.Demand(userIdentity, new FakeRequest()));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<SecurityException>(exception);
    }

    [Permission("Admin")]
    private class FakeRequest { }
}