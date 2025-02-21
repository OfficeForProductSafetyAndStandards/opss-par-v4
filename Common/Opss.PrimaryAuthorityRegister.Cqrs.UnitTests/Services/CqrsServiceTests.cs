using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Opss.PrimaryAuthorityRegister.Http.Services;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using Opss.PrimaryAuthorityRegister.Cqrs;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Test.Queries.Dtos;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Test.Queries;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Test.Commands;

namespace Opss.PrimaryAuthorityRegister.Http.UnitTests.Services;

public class CqrsServiceTests
{
    private static string _baseAddress = "http://api/";

    [Fact]
    public async Task GetAsync_ShouldReturnContent_WhenPutRequestIsSuccessful()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        using var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        };

        HttpRequestMessage? capturedRequest = null;
        string? capturedRequestData = null;

        // Simulate a successful HTTP response
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>(async (request, cancellationToken) =>
            {
                capturedRequest = request; // Capture the request for later inspection
                if (capturedRequest.Content != null)
                    capturedRequestData = await capturedRequest.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(true); // Capture the requset content for later inspection
            })
            .ReturnsAsync(mockResponse);

        using var client = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri(_baseAddress)
        };

        var httpService = new HttpService(client);
        var cqrsService = new CqrsService(httpService);

        var query = new GetTestDataQuery(Guid.NewGuid());

        // Act
        var response = await cqrsService.GetAsync<GetTestDataQuery, TestDataDto>(query).ConfigureAwait(true);

        // Assert
        var expectedUrl = $"{_baseAddress}api?name=GetTestDataQuery";

        Assert.NotNull(response);
        Assert.IsType<HttpObjectResponse<TestDataDto>>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Get, capturedRequest.Method);
        Assert.Equal(expectedUrl, capturedRequest.RequestUri?.ToString());

        // Verify the JSON payload
        var expectedContent = JsonSerializer.Serialize(query);
        Assert.Equal(expectedContent, capturedRequestData);
    }

    [Fact]
    public async Task GetAsync_ShouldThrowException_WhenPutRequestFails()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        using var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("Bad Request")
        };

        // Simulate a failed HTTP response
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);

        using var client = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://api/")
        };

        var httpService = new HttpService(client);
        var cqrsService = new CqrsService(httpService);

        var query = new GetTestDataQuery(Guid.NewGuid());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() => cqrsService.GetAsync<GetTestDataQuery, TestDataDto>(query)).ConfigureAwait(true);
        Assert.Contains("Unknown error occurred", exception.Message, StringComparison.InvariantCulture);
    }

    [Fact]
    public async Task PostAsync_ShouldReturnNoContentResult_WhenPutRequestIsSuccessful()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        using var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        };

        HttpRequestMessage? capturedRequest = null;
        string? capturedRequestData = null;

        // Simulate a successful HTTP response
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>(async (request, cancellationToken) =>
            {
                capturedRequest = request; // Capture the request for later inspection
                if (capturedRequest.Content != null)
                    capturedRequestData = await capturedRequest.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(true); // Capture the requset content for later inspection  
            })
            .ReturnsAsync(mockResponse);

        using var client = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://api/")
        };

        var httpService = new HttpService(client);
        var cqrsService = new CqrsService(httpService);

        var ownerId = Guid.NewGuid();
        var command = new CreateTestDataCommand(ownerId, "Something");

        // Act
        var response = await cqrsService.PostAsync(command).ConfigureAwait(true);

        // Assert
        var expectedUrl = $"{_baseAddress}api?name=CreateTestDataCommand";

        Assert.NotNull(response);
        Assert.IsType<HttpObjectResponse<CreatedResponse>>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.Equal(expectedUrl, capturedRequest.RequestUri?.ToString());

        // Verify the JSON payload
        var expectedContent = JsonSerializer.Serialize(command);
        Assert.Equal(expectedContent, capturedRequestData);
    }

    [Fact]
    public async Task PostAsync_ShouldThrowException_WhenPutRequestFails()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        using var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("Bad Request")
        };

        // Simulate a failed HTTP response
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);

        using var client = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://api/")
        };

        var httpService = new HttpService(client);
        var cqrsService = new CqrsService(httpService);

        var ownerId = Guid.NewGuid();
        var command = new CreateTestDataCommand(ownerId, "Something");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() => cqrsService.PostAsync(command)).ConfigureAwait(true);
        Assert.Contains("Unknown error occurred", exception.Message, StringComparison.InvariantCulture);
    }

    [Fact]
    public async Task PutAsync_ShouldReturnNoContentResult_WhenPutRequestIsSuccessful()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        using var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NoContent
        };

        HttpRequestMessage? capturedRequest = null;
        string? capturedRequestData = null;

        // Simulate a successful HTTP response
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>(async (request, cancellationToken) =>
            {
                capturedRequest = request; // Capture the request for later inspection
                if (capturedRequest.Content != null)
                    capturedRequestData = await capturedRequest.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(true); // Capture the requset content for later inspection
            })
            .ReturnsAsync(mockResponse);

        using var client = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://api/")
        };

        var httpService = new HttpService(client);
        var cqrsService = new CqrsService(httpService);

        var ownerId = Guid.NewGuid();
        var command = new UpdateTestDataCommand(ownerId, Guid.NewGuid(), "Something");

        // Act
        var response = await cqrsService.PutAsync(command).ConfigureAwait(true);

        // Assert
        var expectedUrl = $"{_baseAddress}api?name=UpdateTestDataCommand";

        Assert.NotNull(response);
        Assert.IsType<HttpObjectResponse<NoContentResult>>(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Put, capturedRequest.Method);
        Assert.Equal(expectedUrl, capturedRequest.RequestUri?.ToString());

        // Verify the JSON payload
        var expectedContent = JsonSerializer.Serialize(command);
        Assert.Equal(expectedContent, capturedRequestData);
    }

    [Fact]
    public async Task PutAsync_ShouldThrowException_WhenPutRequestFails()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        using var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("Bad Request")
        };

        // Simulate a failed HTTP response
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);

        using var client = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://api/")
        };

        var httpService = new HttpService(client);
        var cqrsService = new CqrsService(httpService);

        var ownerId = Guid.NewGuid();
        var command = new UpdateTestDataCommand(ownerId, Guid.NewGuid(), "Something");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() => cqrsService.PutAsync(command)).ConfigureAwait(true);
        Assert.Contains("Unknown error occurred", exception.Message, StringComparison.InvariantCulture);
    }
}
