using Opss.PrimaryAuthorityRegister.Web.Application.Entities;
using Opss.PrimaryAuthorityRegister.Web.Application.Problem;
using System.Net;

namespace Opss.PrimaryAuthorityRegister.Web.Application.UnitTests.Entities;

public class HttpObjectResponseTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        var problemDetails = new ProblemDetails(400, "Bad Request");

        // Act
        var objectResponse = new HttpObjectResponse(response, problemDetails);

        // Assert
        Assert.Same(response, objectResponse.Message);
        Assert.Same(problemDetails, objectResponse.Problem);
        Assert.Equal(HttpStatusCode.OK, objectResponse.StatusCode);
        Assert.True(objectResponse.IsSuccessStatusCode);
    }

    [Fact]
    public void Constructor_WithNullProblemDetails_SetsProblemToNull()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        var objectResponse = new HttpObjectResponse(response);

        // Assert
        Assert.Same(response, objectResponse.Message);
        Assert.Null(objectResponse.Problem);
    }
}

public class HttpObjectResponseGenericTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.Created);
        var result = new { Id = 1, Name = "Test" };
        var problemDetails = new ProblemDetails(400, "Bad Request");

        // Act
        var objectResponse = new HttpObjectResponse<object>(response, result, problemDetails);

        // Assert
        Assert.Same(response, objectResponse.Message);
        Assert.Same(result, objectResponse.Result);
        Assert.Same(problemDetails, objectResponse.Problem);
        Assert.Equal(HttpStatusCode.Created, objectResponse.StatusCode);
        Assert.True(objectResponse.IsSuccessStatusCode);
    }

    [Fact]
    public void Constructor_WithNullResultAndProblemDetails_SetsPropertiesToNull()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        var objectResponse = new HttpObjectResponse<object>(response, null, null);

        // Assert
        Assert.Same(response, objectResponse.Message);
        Assert.Null(objectResponse.Result);
        Assert.Null(objectResponse.Problem);
    }
}
