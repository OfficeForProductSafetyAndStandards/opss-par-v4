using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Fakes;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using System.Security.Claims;
using Claim = System.Security.Claims.Claim;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.ClaimChecker_Tests;

public class Given_User_With_Single_Permission_Granted_Which_Command_Requires
{
    [Fact]
    public void When_Claim_Required_Has_Catch_All_Resource_Key_Invoked_Then_Does_Not_Throw_SecurityException()
    {
        // Arrange
        var claim = new Claim(
                "http://www.example.com/Claim/Permissions/Read",
                "http://www.example.com/Claim/Resource/4",
                ClaimTypes.NameIdentifier);

        var catchAllClaim = new Claim(
                "http://www.example.com/Claim/Permissions/Read",
                "*",
                ClaimTypes.NameIdentifier);

        var resourceClaimProvider = new FakeResourceClaimProvider();
        resourceClaimProvider.AddClaimSet(catchAllClaim);

        var claimChecker = new ClaimChecker(resourceClaimProvider);

        // Act
        claimChecker.Demand(new FakeClaimsPrincipal("Authenticated", claim), new FakeRequest());

        // Assert
        Assert.True(true, "No exception should be thrown when demanding required claims.");
    }

    [Fact]
    public void When_Invoked_Then_Does_Not_Throw_SecurityException()
    {
        // Arrange
        var claim = new Claim(
                "http://www.example.com/Claim/Permissions/Read",
                "http://www.example.com/Claim/Resource/4",
                ClaimTypes.NameIdentifier);

        var resourceClaimProvider = new FakeResourceClaimProvider();
        resourceClaimProvider.AddClaimSet(claim);

        var claimChecker = new ClaimChecker(resourceClaimProvider);

        // Act
        claimChecker.Demand(new FakeClaimsPrincipal("Authenticated", claim), new FakeRequest());

        // Assert
        Assert.True(true, "No exception should be thrown when demanding required claims.");
    }

    [AllowAnonymous]
    private class FakeRequest { }
}