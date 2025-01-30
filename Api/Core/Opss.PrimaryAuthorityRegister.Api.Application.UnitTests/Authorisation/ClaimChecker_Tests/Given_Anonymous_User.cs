using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Fakes;
using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using System.Security;
using System.Security.Principal;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.ClaimChecker_Tests;

public class Given_Anonymous_User
{
    [Fact]
    public void When_Invoked_On_Context_With_No_AllowAnonymous_Attribute_Then_Throws_ClaimRequiredAttributeNotFoundException()
    {
        // Arrange
        var nonAuthenticatedPrincipal = new GenericPrincipal(new GenericIdentity(""), Array.Empty<string>());

        var claimChecker = new ClaimChecker(new FakeResourceClaimProvider());

        // Act
        var exception =
                Record.Exception(
                                 () => claimChecker.Demand(nonAuthenticatedPrincipal, new object()));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ClaimRequiredAttributeNotFoundException>(exception);
    }
    [Fact]
    public void When_Invoked_On_Context_With_MustBeAuthenticated_Attribute_Then_Throws_SecurityException()
    {
        // Arrange
        var nonAuthenticatedPrincipal = new GenericPrincipal(new GenericIdentity(""), Array.Empty<string>());

        var claimChecker = new ClaimChecker(new FakeResourceClaimProvider());

        // Act
        var exception =
                Record.Exception(
                                 () => claimChecker.Demand(nonAuthenticatedPrincipal, new MustBeAuthenticatedContext()));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<SecurityException>(exception);
    }

    [Fact]
    public void When_Invoked_On_Context_With_AllowAnonymous_Attribute_Then_Does_Not_Throw_SecurityException()
    {
        // Arrange
        var nonAuthenticatedPrincipal = new GenericPrincipal(new GenericIdentity(""), Array.Empty<string>());

        var claimChecker = new ClaimChecker(new FakeResourceClaimProvider());

        // Act
        claimChecker.Demand(nonAuthenticatedPrincipal, new AllowAnonymousContext());

        // Assert
        Assert.True(true, "No exception should be raised from the Demand call.");
    }

    [AllowAnonymous]
    private class AllowAnonymousContext
    {
    }

    [MustBeAuthenticated]
    private class MustBeAuthenticatedContext
    {
    }
}