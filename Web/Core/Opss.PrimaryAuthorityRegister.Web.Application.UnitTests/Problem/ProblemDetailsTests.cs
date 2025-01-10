using Opss.PrimaryAuthorityRegister.Web.Application.Problem;
namespace Opss.PrimaryAuthorityRegister.Web.Application.UnitTests.Problem;

public class ProblemDetailsTests
{
    [Fact]
    public void Constructor_WithStatusAndDetail_SetsPropertiesCorrectly()
    {
        // Arrange
        int status = 404;
        string detail = "Resource not found.";

        // Act
        var problemDetails = new ProblemDetails(status, detail);

        // Assert
        Assert.Equal("Error", problemDetails.Title);
        Assert.Equal(status, problemDetails.Status);
        Assert.Equal(detail, problemDetails.Detail);
        Assert.Null(problemDetails.Type);
        Assert.Null(problemDetails.TraceId);
        Assert.Null(problemDetails.Errors);
        Assert.Null(problemDetails.ExceptionDetails);
    }

    [Fact]
    public void ThrowException_ThrowsProblemDetailsException()
    {
        // Arrange
        var problemDetails = new ProblemDetails(500, "Internal server error.");

        // Act & Assert
        var exception = Assert.Throws<ProblemDetailsException>(() => problemDetails.ThrowException());
        Assert.Equal("Exception of type 'Opss.PrimaryAuthorityRegister.Web.Application.Problem.ProblemDetailsException' was thrown.", exception.Message);
    }

    [Fact]
    public void ThrowException_WithExceptionDetails_ThrowsWithCorrectMessage()
    {
        // Arrange
        var problemDetails = new ProblemDetails(400, "Validation error.")
        {
            ExceptionDetails = new[]
            {
                new ProblemDetailsExceptionDetails
                {
                    Raw = "Validation failed due to invalid input."
                }
            }
        };

        // Act & Assert
        var exception = Assert.Throws<ProblemDetailsException>(() => problemDetails.ThrowException());
        Assert.Equal("Validation failed due to invalid input.", exception.Message);
    }

    [Fact]
    public void Constructor_WithNullExceptionDetails_SetsExceptionMessageToNull()
    {
        // Arrange
        var problemDetails = new ProblemDetails(500, "Some error.")
        {
            ExceptionDetails = null
        };

        // Act & Assert
        var exception = Assert.Throws<ProblemDetailsException>(() => problemDetails.ThrowException());
        Assert.Equal("Exception of type 'Opss.PrimaryAuthorityRegister.Web.Application.Problem.ProblemDetailsException' was thrown.", exception.Message);
    }

    private static readonly string[] value = new[] { "Error1", "Error2" };

    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var errors = new Dictionary<string, string[]> { { "Field", value } };
        var exceptionDetails = new[]
        {
            new ProblemDetailsExceptionDetails
            {
                Message = "Test exception",
                Type = "ValidationException",
                Raw = "Validation error occurred."
            }
        };

        var problemDetails = new ProblemDetails(400, "Validation error.")
        {
            Type = "https://example.com/problem",
            TraceId = "12345",
            Errors = errors,
            ExceptionDetails = exceptionDetails
        };

        // Act & Assert
        Assert.Equal("https://example.com/problem", problemDetails.Type);
        Assert.Equal("12345", problemDetails.TraceId);
        Assert.Same(errors, problemDetails.Errors);
        Assert.Same(exceptionDetails, problemDetails.ExceptionDetails);
    }
}
