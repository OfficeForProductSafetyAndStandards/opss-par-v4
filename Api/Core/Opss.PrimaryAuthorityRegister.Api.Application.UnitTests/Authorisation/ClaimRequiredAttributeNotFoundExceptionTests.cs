using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation;

public class ClaimRequiredAttributeNotFoundExceptionTests
{
    [Fact]
    public void ClaimRequiredAttributeNotFoundException_WhenBlankConstructor_DefaultMessageReturned()
    {
        // Act
        var exception = new ClaimRequiredAttributeNotFoundException();

        // Assert
        Assert.Equal("The expected attribute is missing.", exception.Message);
    }

    [Fact]
    public void ClaimRequiredAttributeNotFoundException_WhenMessageProvided_MessageReturned()
    {
        // Arrange
        var message = "My exception Message";

        // Act
        var exception = new ClaimRequiredAttributeNotFoundException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void ClaimRequiredAttributeNotFoundException_WhenObjectProvided_MessageReturned()
    {
        // Arrange
        var resource = new ClaimRequiredAttributeNotFoundExceptionTests();
        var message = $"The expected attribute was missing from type: {resource.GetType().Name}";

        // Act
        var exception = new ClaimRequiredAttributeNotFoundException(resource);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void ClaimRequiredAttributeNotFoundException_WhenMessageAndInnerExceptionProvided_MessageReturned()
    {
        // Arrange
        var message = "My exception Message";
        var innerMessage = "My inner message";

        var inner = new FormatException(innerMessage);

        // Act
        var exception = new ClaimRequiredAttributeNotFoundException(message, inner);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(inner, exception.InnerException);
    }
}
