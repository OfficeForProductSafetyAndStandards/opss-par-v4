using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.ClaimRequiredAttribute_Tests;

public class Given_Type_Decorated_With_Plain_ClaimRequiredAttribute
{
    private const string ClaimType = "http://www.example.com/Claims/Permission";
    private const string Resource = "MyResource/73632";
    private const string Right = "ReadMe";

    [Fact]
    public void When_Getting_Claim_Then_ClaimType_Is_Correctly_Populated()
    {
        // Arrange
        var sampleResource = new SampleResource();
        var claimRequiredAttribute =
                typeof(SampleResource).GetCustomAttributes(typeof(ClaimRequiredAttribute), false).Cast
                        <ClaimRequiredAttribute>().Single();

        // Act
        var claim = claimRequiredAttribute.GetClaim(sampleResource);

        // Assert
        Assert.Equal(ClaimType, claim.Type);
    }

    [Fact]
    public void When_Getting_Claim_Then_Resource_Is_Correctly_Populated()
    {
        // Arrange
        var sampleResource = new SampleResource();
        var claimRequiredAttribute =
                typeof(SampleResource).GetCustomAttributes(typeof(ClaimRequiredAttribute), false).Cast
                        <ClaimRequiredAttribute>().Single();

        // Act
        var claim = claimRequiredAttribute.GetClaim(sampleResource);

        // Assert
        Assert.Equal(Resource, claim.Value);
    }

    [Fact]
    public void When_Getting_Claim_Then_Right_Is_Correctly_Populated()
    {
        // Arrange
        var sampleResource = new SampleResource();
        var claimRequiredAttribute =
                typeof(SampleResource).GetCustomAttributes(typeof(ClaimRequiredAttribute), false).Cast
                        <ClaimRequiredAttribute>().Single();

        // Act
        var claim = claimRequiredAttribute.GetClaim(sampleResource);

        // Assert
        Assert.Equal(Right, claim.ValueType);
    }

    [ClaimRequired(ClaimType, Resource, Right)]
    private class SampleResource
    {
    }
}