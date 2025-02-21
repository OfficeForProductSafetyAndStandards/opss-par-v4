using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.AttributeBasedResourceClaimProvider_Tests;

public class Given_Message_With_Multiple_Required_Claims_In_Different_Sets
{
    private const string PermissionType = "http://www.example.com/Claims/Types/Permission";
    private const string ResourceKey = "SomeResourceType/6736";

    [Fact]
    public void When_GetRequiredClaims_Then_Multiple_PossibleClaimSet_Returned()
    {
        // Arrange
        var attributeBasedResourceClaimProvider =
                new AttributeBasedResourceClaimProvider(Enumerable.Empty<IResourceKeyExpander>());

        // Act
        var result =
                attributeBasedResourceClaimProvider.GetDemandedClaims(new FakeCommandWithMultipleClaimsInDifferentSet());

        // Assert
        Assert.NotNull(result.GetRequiredClaimSet("Set1"));
        Assert.NotNull(result.GetRequiredClaimSet("Set2"));
    }

    [Fact]
    public void When_GetRequiredClaims_Then_Multiple_PossibleClaimSets_Contains_All_Claims_Specified_In_Attributes()
    {
        // Arrange
        var attributeBasedResourceClaimProvider =
                new AttributeBasedResourceClaimProvider(Enumerable.Empty<IResourceKeyExpander>());

        // Act
        var result =
                attributeBasedResourceClaimProvider.GetDemandedClaims(new FakeCommandWithMultipleClaimsInDifferentSet());

        // Assert
        Assert.True(result?.GetRequiredClaimSet("Set1")?
                            .IsSatisfiedBy([new Claim(PermissionType, ResourceKey, "Read")]));

        Assert.True(result?.GetRequiredClaimSet("Set2")?
                            .IsSatisfiedBy([new Claim(PermissionType, ResourceKey, "Write")]));
    }

    [Fact]
    public void When_GetRequiredClaims_With_Common_Attribute_Then_All_Sets_Contain_Common_ClaimRequired()
    {
        // Arrange
        var attributeBasedResourceClaimProvider =
                new AttributeBasedResourceClaimProvider(Enumerable.Empty<IResourceKeyExpander>());

        // Act
        var result =
                attributeBasedResourceClaimProvider.GetDemandedClaims(new FakeCommandWithMultipleClaimsInDifferentSetWithCommonClaim());

        // Assert
        Assert.True(result?.GetRequiredClaimSet("Set1")?
                            .IsSatisfiedBy([
                                            new Claim(PermissionType, ResourceKey, "Read"),
                                            new Claim(PermissionType, ResourceKey, "CommonPermission") ]));

        Assert.True(result?.GetRequiredClaimSet("Set2")?
                            .IsSatisfiedBy([
                                            new Claim(PermissionType, ResourceKey, "Write"),
                                            new Claim(PermissionType, ResourceKey, "CommonPermission") ]));
    }

    [ClaimRequired(PermissionType, ResourceKey, "Read", Group = "Set1")]
    [ClaimRequired(PermissionType, ResourceKey, "Write", Group = "Set2")]
    private class FakeCommandWithMultipleClaimsInDifferentSet : ICommand
    {
    }

    [ClaimRequired(PermissionType, ResourceKey, "Read", Group = "Set1")]
    [ClaimRequired(PermissionType, ResourceKey, "Write", Group = "Set2")]
    [ClaimRequired(PermissionType, ResourceKey, "CommonPermission")]
    private class FakeCommandWithMultipleClaimsInDifferentSetWithCommonClaim : ICommand
    {
    }
}