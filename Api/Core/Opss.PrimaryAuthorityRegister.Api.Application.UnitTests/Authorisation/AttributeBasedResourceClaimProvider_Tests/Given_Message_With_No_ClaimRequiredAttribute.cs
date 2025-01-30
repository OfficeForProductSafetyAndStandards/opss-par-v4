using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.AttributeBasedResourceClaimProvider_Tests;

public class Given_Message_With_No_ClaimRequiredAttribute
{
    [Fact]
    public void When_GetRequiredClaims_Called_Then_Throws_ClaimRequiredAttributeNotFoundException()
    {
        // Arrange
        var attributeBasedResourceClaimProvider =
                new AttributeBasedResourceClaimProvider(Enumerable.Empty<IResourceKeyExpander>());

        // Act
        var result =
                Record.Exception(
                                 () =>
                                 attributeBasedResourceClaimProvider.
                                         GetDemandedClaims(
                                                         new FakeCommandWithNoClaimRequiredAttribute
                                                                ()));

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ClaimRequiredAttributeNotFoundException>(result);
    }

    public class FakeCommandWithNoClaimRequiredAttribute : ICommand
    {
    }
}