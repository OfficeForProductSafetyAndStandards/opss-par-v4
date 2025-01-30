using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Fakes;
using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using System.Security.Claims;
using Claim = System.Security.Claims.Claim;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.ClaimChecker_Tests
{
    public class Given_User_With_Multiple_Permissions_Granted_Which_Command_Requires
    {
        [Fact]
        public void When_Invoked_Then_Does_Not_Throw_SecurityException()
        {
            // Arrange
            var claim1 = new Claim(
                    "Read",
                    "Resource/4",
                    ClaimTypes.NameIdentifier);
            var claim2 = new Claim(
                    "Read",
                    "Resource/4",
                    ClaimTypes.NameIdentifier);

            var resourceClaimProvider = new FakeResourceClaimProvider();
            resourceClaimProvider.AddClaimSet(claim1, claim2);

            var claimChecker = new ClaimChecker(resourceClaimProvider);

            // Act
            claimChecker.Demand(new FakeClaimsPrincipal("Authenticated", claim1, claim2), new FakeRequest());

            // Assert
            Assert.True(true, "No exception should be thrown when demanding required claims.");
        }

        [AllowAnonymous]
        private class FakeRequest { }
    }
}