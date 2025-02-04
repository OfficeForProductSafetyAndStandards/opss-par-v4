using Opss.PrimaryAuthorityRegister.Common.Problem;
using System.Net;

namespace Opss.PrimaryAuthorityRegister.Common.UnitTests.Problem;

public class ProblemDetailsTests
{
    [Fact]
    public void Constructor_WithException_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var exception = new InvalidOperationException("Something went wrong");
        var status = HttpStatusCode.InternalServerError;

        // Act
        var problemDetails = new ProblemDetails(status, exception);

        // Assert
        Assert.Equal(status, problemDetails.Status);
        Assert.Equal("Something went wrong", problemDetails.Detail);
        Assert.Equal(exception.StackTrace, problemDetails.StackTrace);
        Assert.Equal("InvalidOperationException", problemDetails.Type);
    }

    [Fact]
    public void Constructor_WithNullException_ShouldSetDefaultDetail()
    {
        // Arrange
        var status = HttpStatusCode.BadRequest;

        // Act
        var problemDetails = new ProblemDetails(status, null);

        // Assert
        Assert.Equal(status, problemDetails.Status);
        Assert.Equal("An error occured. Please try again later.", problemDetails.Detail);
        Assert.Null(problemDetails.StackTrace);
        Assert.Null(problemDetails.Type);
    }
}
