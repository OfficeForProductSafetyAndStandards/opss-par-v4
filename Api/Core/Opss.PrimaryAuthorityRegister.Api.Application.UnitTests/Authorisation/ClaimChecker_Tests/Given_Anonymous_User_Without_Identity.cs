using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Fakes;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.ClaimChecker_Tests;

public class Given_Anonymous_User_Without_Identity
{
    [Fact]
    public void When_Invoked_On_Context_With_MustBeAuthenticated_Attribute_Then_Throws_ClaimRequiredAttributeNotFoundException()
    {
        // Arrange
        var claimsIdentity = new ClaimsIdentity();
        var nonAuthenticatedPrincipal = new ClaimsPrincipal(claimsIdentity);

        var claimChecker = new ClaimChecker(new FakeResourceClaimProvider());

        // Act
        var exception =
                Record.Exception(
                                 () => claimChecker.Demand(nonAuthenticatedPrincipal, new MustBeAuthenticatedContext()));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<SecurityException>(exception);
    }

    [MustBeAuthenticated]
    private class MustBeAuthenticatedContext
    {
    }
}