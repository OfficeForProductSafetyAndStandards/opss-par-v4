using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;

namespace Opss.PrimaryAuthorityRegister.Cqrs.UnitTests.AuthorisationAttributes;

public class ClaimRequiredAttributeTests
{

    private const string ClaimType = "http://www.example.com/Claims/Permission";
    private const string Right = "ReadMe";

    [Fact]
    public void GetClaim_Throws_InvalidOperationException_WhenInvalidPropertyPlaceholderUsed()
    {
        // Arrange
        var sampleResource = new InvalidPropertySampleResource { MyId = 5451584 };
        var claimRequiredAttribute =
                typeof(InvalidPropertySampleResource).GetCustomAttributes(typeof(ClaimRequiredAttribute), false).Cast
                        <ClaimRequiredAttribute>().Single();

        // Act
        var exception = Record.Exception(() => claimRequiredAttribute.GetClaim(sampleResource));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [ClaimRequired(ClaimType, "/SampleResource/{APropertyThatDoesNotExist}", Right)]
    private sealed class InvalidPropertySampleResource
    {
        public int MyId { get; set; }
    }

    [Fact]
    public void GetClaim_ReturnsCorrectClaimType_WhenCorrectAttributeSpecified()
    {
        // Arrange
        var sampleResource = new ValidPropertySampleResource { MyId = 5451584 };
        var claimRequiredAttribute =
                typeof(ValidPropertySampleResource).GetCustomAttributes(typeof(ClaimRequiredAttribute), false).Cast
                        <ClaimRequiredAttribute>().Single();

        // Act
        var claim = claimRequiredAttribute.GetClaim(sampleResource);

        // Assert
        Assert.Equal(ClaimType, claim.Type);
    }

    [Fact]
    public void GetClaim_ReturnsCorrectClaimValue_WhenCorrectAttributeSpecified()
    {
        // Arrange
        var sampleResource = new ValidPropertySampleResource { MyId = 5451584 };
        var claimRequiredAttribute =
                typeof(ValidPropertySampleResource).GetCustomAttributes(typeof(ClaimRequiredAttribute), false).Cast
                        <ClaimRequiredAttribute>().Single();

        // Act
        var claim = claimRequiredAttribute.GetClaim(sampleResource);

        // Assert
        Assert.Equal("/SampleResource/5451584", claim.Value);
    }

    [Fact]
    public void GetClaim_ReturnsCorrectClaimValueType_WhenCorrectAttributeSpecified()
    {
        // Arrange
        var sampleResource = new ValidPropertySampleResource { MyId = 5451584 };
        var claimRequiredAttribute =
                typeof(ValidPropertySampleResource).GetCustomAttributes(typeof(ClaimRequiredAttribute), false).Cast
                        <ClaimRequiredAttribute>().Single();

        // Act
        var claim = claimRequiredAttribute.GetClaim(sampleResource);

        // Assert
        Assert.Equal(Right, claim.ValueType);
    }

    [ClaimRequired(ClaimType, "/SampleResource/{MyId}", Right)]
    private sealed class ValidPropertySampleResource
    {
        public int MyId { get; set; }
    }
}
