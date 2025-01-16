using Opss.PrimaryAuthorityRegister.Web.Application.Problem;

namespace Opss.PrimaryAuthorityRegister.Web.Application.UnitTests.Problem;

public class ProblemDetailsExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateInstance()
    {
        // Act
        var exception = new ProblemDetailsException();

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Exception of type '{typeof(ProblemDetailsException).FullName}' was thrown.", exception.Message);
    }

    [Fact]
    public void Constructor_WithNullProblemDetails_ShouldCreateInstanceWithNullMessage()
    {
        // Act
        var exception = new ProblemDetailsException(null);

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Exception of type '{typeof(ProblemDetailsException).FullName}' was thrown.", exception.Message);
    }

    [Fact]
    public void Constructor_WithProblemDetailsHavingNoExceptionDetails_ShouldCreateInstanceWithNullMessage()
    {
        // Arrange
        var problemDetails = new ProblemDetails(400, "A problem occurred")
        {
            ExceptionDetails = null
        };

        // Act
        var exception = new ProblemDetailsException(problemDetails);

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Exception of type '{typeof(ProblemDetailsException).FullName}' was thrown.", exception.Message);
    }

    [Fact]
    public void Constructor_WithProblemDetailsHavingEmptyExceptionDetails_ShouldCreateInstanceWithNullMessage()
    {
        // Arrange
        var problemDetails = new ProblemDetails(400, "A problem occurred")
        {
            ExceptionDetails = Array.Empty<ProblemDetailsExceptionDetails>()
        };

        // Act
        var exception = new ProblemDetailsException(problemDetails);

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Exception of type '{typeof(ProblemDetailsException).FullName}' was thrown.", exception.Message);
    }

    [Fact]
    public void Constructor_WithProblemDetailsHavingExceptionDetails_ShouldUseFirstRawAsMessage()
    {
        // Arrange
        var rawMessage = "{ \"error\": \"Invalid data\" }";
        var problemDetails = new ProblemDetails(400, "A problem occurred")
        {
            ExceptionDetails = new[]
            {
                new ProblemDetailsExceptionDetails { Raw = rawMessage },
                new ProblemDetailsExceptionDetails { Raw = "Another error" }
            }
        };

        // Act
        var exception = new ProblemDetailsException(problemDetails);

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(rawMessage, exception.Message);
    }

    [Fact]
    public void Constructor_WithProblemDetailsHavingExceptionDetails_WithNullRaw_ShouldCreateInstanceWithNullMessage()
    {
        // Arrange
        var problemDetails = new ProblemDetails(400, "A problem occurred")
        {
            ExceptionDetails = new[]
            {
                new ProblemDetailsExceptionDetails { Raw = null }
            }
        };

        // Act
        var exception = new ProblemDetailsException(problemDetails);

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Exception of type '{typeof(ProblemDetailsException).FullName}' was thrown.", exception.Message);
    }

    [Fact]
    public void ProblemDetails_ThrowException_ShouldThrowProblemDetailsException()
    {
        // Arrange
        var rawMessage = "{ \"error\": \"Invalid data\" }";
        var problemDetails = new ProblemDetails(500, "Internal Server Error")
        {
            ExceptionDetails = new[]
            {
                new ProblemDetailsExceptionDetails { Raw = rawMessage }
            }
        };

        // Act & Assert
        var exception = Assert.Throws<ProblemDetailsException>(() => problemDetails.ThrowException());
        Assert.NotNull(exception);
        Assert.Equal(rawMessage, exception.Message);
    }
}
