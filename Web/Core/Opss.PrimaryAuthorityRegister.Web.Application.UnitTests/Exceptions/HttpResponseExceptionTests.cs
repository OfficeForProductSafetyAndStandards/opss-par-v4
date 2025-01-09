using Opss.PrimaryAuthorityRegister.Web.Application.Exceptions;
using System.Net;

namespace Opss.PrimaryAuthorityRegister.Web.Application.UnitTests.Exceptions;

public class HttpResponseExceptionTests
{
    [Fact]
    public void Constructor_WithStatusCodeAndContent_SetsPropertiesCorrectly()
    {
        // Arrange
        var statusCode = HttpStatusCode.BadRequest;
        var content = "Bad request error.";

        // Act
        var exception = new HttpResponseException(statusCode, content);

        // Assert
        Assert.NotNull(exception.Response);
        Assert.Equal(statusCode, exception.Response.StatusCode);
        Assert.Equal(content, exception.Message);
    }

    [Fact]
    public void Constructor_WithHttpResponseMessageAndContent_SetsPropertiesCorrectly()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            ReasonPhrase = "Internal Server Error"
        };
        var content = "An internal server error occurred.";

        // Act
        var exception = new HttpResponseException(response, content);

        // Assert
        Assert.NotNull(exception.Response);
        Assert.Same(response, exception.Response);
        Assert.Equal(content, exception.Message);
    }

    [Fact]
    public void Constructor_WithNullHttpResponseMessage_ThrowsArgumentNullException()
    {
        // Arrange
        HttpResponseMessage? response = null;
        var content = "Content should not matter.";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new HttpResponseException(response!, content));
        Assert.Equal("response", exception.ParamName);
    }
}
