using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.AttributeBasedResourceClaimProvider_Tests;

public class Given_Message_With_Single_Required_Claim
{
    private const string PermissionType = "http://www.example.com/Claims/Types/Permission";
    private const string ResourceKey = "SomeResourceType/6736";

    [Fact]
    public void When_GetRequiredClaims_Then_Single_PossibleClaimSet_Contains_All_Claims_Specified_In_Attributes()
    {
        // Arrange
        var attributeBasedResourceClaimProvider =
                new AttributeBasedResourceClaimProvider(Enumerable.Empty<IResourceKeyExpander>());

        // Act
        var result = attributeBasedResourceClaimProvider.GetDemandedClaims(new FakeCommandWithSingleClaim());

        // Assert
        Assert.True(
                    result.GetRequiredClaimSet("Default").IsSatisfiedBy(new[] { new Claim(
                                                                                        PermissionType, ResourceKey, "Read") }));
    }

    [Fact]
    public void When_GetRequiredClaims_Then_Single_PossibleClaimSet_Returned()
    {
        // Arrange
        var attributeBasedResourceClaimProvider =
                new AttributeBasedResourceClaimProvider(Enumerable.Empty<IResourceKeyExpander>());

        // Act
        var result = attributeBasedResourceClaimProvider.GetDemandedClaims(new FakeCommandWithSingleClaim());

        // Assert
        Assert.NotNull(result.GetRequiredClaimSet("Default"));
    }

    [ClaimRequired(PermissionType, ResourceKey, "Read")]
    private class FakeCommandWithSingleClaim : ICommand
    {
    }
}