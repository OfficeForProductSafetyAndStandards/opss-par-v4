using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.AttributeBasedResourcePermissionProvider_Tests
{
    public class Given_Message_With_Multiple_Possible_Permissions
    {

        [Fact]
        public void When_GetRequiredClaims_Then_Single_PossibleClaimSet_Returned()
        {
            // Arrange
            var attributeBasedResourceClaimProvider =
                    new AttributeBasedResourceClaimProvider(new List<IResourceKeyExpander> { new MultiplePermissionResourceKeyExpander() });

            // Act
            var result = attributeBasedResourceClaimProvider.GetDemandedClaims(new FakeCommandWithMultiplePossiblePermissions());

            // Assert
            var defaultGroup = result.GetRequiredClaimSet("Default");
            var expectedClaims = new[] {
                new Claim("urn:claims/permission", "*", "System Admin"),
                new Claim("urn:claims/permission", "*", "Office Admin")
            };

            Assert.True(defaultGroup.IsSatisfiedBy(expectedClaims));
        }

        [Permission("System Admin")]
        private class FakeCommandWithMultiplePossiblePermissions : ICommand
        {
        }
    }
}
