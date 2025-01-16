using Opss.PrimaryAuthorityRegister.Web.Application.Problem;
using Xunit;

namespace Opss.PrimaryAuthorityRegister.Web.Application.UnitTests.Problem;

public class ProblemDetailsExceptionDetailsTests
{
    [Fact]
    public void Properties_ShouldAllowSettingAndGettingValues()
    {
        // Arrange
        var details = new ProblemDetailsExceptionDetails();
        var message = "An error occurred.";
        var type = "ValidationError";
        var raw = "{ \"error\": \"Invalid data\" }";

        // Act
        details.Message = message;
        details.Type = type;
        details.Raw = raw;

        // Assert
        Assert.Equal(message, details.Message);
        Assert.Equal(type, details.Type);
        Assert.Equal(raw, details.Raw);
    }

    [Fact]
    public void DefaultProperties_ShouldBeNull_WhenInstantiated()
    {
        // Arrange
        var details = new ProblemDetailsExceptionDetails();

        // Act & Assert
        Assert.Null(details.Message);
        Assert.Null(details.Type);
        Assert.Null(details.Raw);
    }
}

