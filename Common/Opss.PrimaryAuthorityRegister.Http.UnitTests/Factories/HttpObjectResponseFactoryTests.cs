using System.Net;
using System.Text.Json;
using System.Security.Authentication;
using Opss.PrimaryAuthorityRegister.Http.Factories;
using Opss.PrimaryAuthorityRegister.Http.Problem;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;

namespace Opss.PrimaryAuthorityRegister.Http.UnitTests.Factories;

public class HttpObjectResponseFactoryTests
{
    [Fact]
    public async Task DetermineSuccess_Generic_SuccessfulResponse_ReturnsObjectResponseWithResult()
    {
        // Arrange
        var responseContent = "{\"Name\":\"Test\",\"Value\":42}";
        using var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent)
        };

        // Act
        var result = await HttpObjectResponseFactory.DetermineSuccess<TestObject>(responseMessage).ConfigureAwait(true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("Test", result.Result.Name);
        Assert.Equal(42, result.Result.Value);
    }
    [Fact]
    public async Task DetermineSuccess_Generic_SuccessfulResponse_ReturnsStringResponseWithResult()
    {
        // Arrange
        var responseContent = "Content";
        using var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent)
        };

        // Act
        var result = await HttpObjectResponseFactory.DetermineSuccess<string>(responseMessage).ConfigureAwait(true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("Content", result.Result);
    }

    [Fact]
    public async Task DetermineSuccess_Generic_ErrorResponse_ThrowsHttpResponseException()
    {
        // Arrange
        var problemDetails = new ProblemDetails(HttpStatusCode.BadRequest, new ApplicationException("Bad Request"));
        var responseContent = JsonSerializer.Serialize(problemDetails);
        using var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(responseContent)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(
            async () => await HttpObjectResponseFactory.DetermineSuccess<TestObject>(responseMessage).ConfigureAwait(true)).ConfigureAwait(true);

        Assert.Equal(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        Assert.Equal(problemDetails.Detail, exception.Message);
    }

    [Fact]
    public async Task DetermineSuccess_Generic_ShouldThrowHttpResponseException_WhenJsonDeserializationFails()
    {
        // Arrange
        using var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("{ Invalid JSON string"), // Simulate invalid JSON body
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(async () =>
            await HttpObjectResponseFactory.DetermineSuccess<TestObject>(httpResponseMessage).ConfigureAwait(true)).ConfigureAwait(true);

        Assert.Equal(HttpStatusCode.InternalServerError, exception.Response.StatusCode);
        Assert.StartsWith("'I' is an invalid start of a property name.", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DetermineSuccess_NonGeneric_SuccessfulResponse_ReturnsObjectResponse()
    {
        // Arrange
        using var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(string.Empty)
        };

        // Act
        var result = await HttpObjectResponseFactory.DetermineSuccess(responseMessage).ConfigureAwait(true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task DetermineSuccess_NonGeneric_ErrorResponse_ThrowsHttpResponseException()
    {
        // Arrange
        var problemDetails = new ProblemDetails(HttpStatusCode.InternalServerError, new ApplicationException("Internal Server Error"));
        var responseContent = JsonSerializer.Serialize(problemDetails);
        using var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent(responseContent)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(
            async () => await HttpObjectResponseFactory.DetermineSuccess(responseMessage).ConfigureAwait(true)).ConfigureAwait(true);

        Assert.Equal(HttpStatusCode.InternalServerError, exception.Response.StatusCode);
        Assert.Equal(problemDetails.Detail, exception.Message);
    }

    [Fact]
    public async Task DetermineSuccess_NonGeneric_ShouldThrowHttpResponseException_WhenJsonDeserializationFails()
    {
        // Arrange
        using var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("{ Invalid JSON string"), // Simulate invalid JSON body
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(async () =>
            await HttpObjectResponseFactory.DetermineSuccess(httpResponseMessage).ConfigureAwait(true)).ConfigureAwait(true);

        Assert.Equal(HttpStatusCode.InternalServerError, exception.Response.StatusCode);
        Assert.StartsWith("'I' is an invalid start of a property name.", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Problem_Generic_CreatesObjectResponseWithProblemDetails()
    {
        // Arrange
        var problemDetails = new ProblemDetails(HttpStatusCode.NotFound, new FileNotFoundException("Not Found"));

        // Act
        var result = HttpObjectResponseFactory.Problem<TestObject>(problemDetails);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal(problemDetails, result.Problem);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Problem_NonGeneric_CreatesObjectResponseWithProblemDetails()
    {
        // Arrange
        var problemDetails = new ProblemDetails(HttpStatusCode.Forbidden, new AuthenticationException("Forbidden"));

        // Act
        var result = HttpObjectResponseFactory.Problem(problemDetails);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        Assert.Equal(problemDetails, result.Problem);
    }

    private class TestObject
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Minor Code Smell",
            "S3459:Unassigned members should be removed",
            Justification = "Accessed through JSON Deserialization")]
        public string? Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Minor Code Smell",
            "S3459:Unassigned members should be removed",
            Justification = "Accessed through JSON Deserialization")]
        public int? Value { get; set; }
    }
}

