using Opss.PrimaryAuthorityRegister.Api.Persistence.Exceptions;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.UnitTests.Exceptions;

public class ServiceNotFoundExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessage_WhenSingleArgumentIsProvided()
    {
        // Arrange
        var serviceName = "TestService";

        // Act
        var exception = new ServiceNotFoundException(serviceName);

        // Assert
        Assert.Equal($"Service: {serviceName} not found", exception.Message);
    }

    [Fact]
    public void Constructor_ShouldSetMessageAndInnerException_WhenTwoArgumentsAreProvided()
    {
        // Arrange
        var serviceName = "TestService";
        var innerException = new InvalidOperationException("Inner exception message");

        // Act
        var exception = new ServiceNotFoundException(serviceName, innerException);

        // Assert
        Assert.Equal($"Service: {serviceName} not found", exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }
}

