using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.ClaimRequiredAttribute_Tests;

public class Given_Type_Decorated_With_ClaimRequiredAttribute_Using_Invalid_Property_Placeholder
{
    private const string ClaimType = "http://www.example.com/Claims/Permission";
    private const string Right = "ReadMe";

    [Fact]
    public void When_Getting_Claim_Then_Throws_InvalidOperationException()
    {
        // Arrange
        var sampleResource = new SampleResource { MyId = 5451584 };
        var claimRequiredAttribute =
                typeof(SampleResource).GetCustomAttributes(typeof(ClaimRequiredAttribute), false).Cast
                        <ClaimRequiredAttribute>().Single();

        // Act
        var exception =
                Record.Exception(
                                 () => claimRequiredAttribute.GetClaim(sampleResource));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [ClaimRequired(ClaimType, "/SampleResource/{APropertyThatDoesNotExist}", Right)]
    private class SampleResource
    {
        public int MyId { get; set; }
    }
}