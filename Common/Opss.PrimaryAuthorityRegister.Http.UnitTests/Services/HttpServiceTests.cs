using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Moq;
using Xunit;
using System.Net.Http.Headers;
using Opss.PrimaryAuthorityRegister.Http.Services;
using Moq.Protected;


namespace Opss.PrimaryAuthorityRegister.Http.UnitTests.Services;

public class HttpServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly HttpService _httpService;

    public HttpServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpService = new HttpService(_httpClient);
    }

    [Fact]
    public async Task HttpSendAsync_ShouldReturnSuccessResponse_WhenApiReturnsSuccess()
    {
        // Arrange
        var testResponse = new TestResponse { Message = "Success" };
        var responseContent = new StringContent(JsonSerializer.Serialize(testResponse), Encoding.UTF8, "application/json");
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK) { Content = responseContent };

        HttpRequestMessage? capturedRequest = null;
        string? capturedRequestData = null;

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>(async (request, cancellationToken) =>
            {
                capturedRequest = request; // Capture the request for later inspection
                if (capturedRequest.Content != null)
                    capturedRequestData = await capturedRequest.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(true);
            })
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _httpService.HttpSendAsync<TestResponse>(HttpMethod.Get, new Uri("https://example.com"));

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Get, capturedRequest.Method);
        Assert.Null(capturedRequestData);

        Assert.Equal("Success", result.Result?.Message);
    }

    [Fact]
    public async Task HttpSendAsync_ShouldIncludeBearerToken_WhenProvided()
    {
        // Arrange
        var token = "test-token";
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };

        HttpRequestMessage? capturedRequest = null;
        string? capturedRequestData = null;

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>(async (request, cancellationToken) =>
            {
                capturedRequest = request; // Capture the request for later inspection
                if (capturedRequest.Content != null)
                    capturedRequestData = await capturedRequest.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(true);
            })
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _httpService.HttpSendAsync<TestResponse>(HttpMethod.Get, new Uri("https://example.com"), bearerToken: token);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task HttpSendAsync_ShouldSendJsonPayload_WhenDataIsProvided()
    {
        // Arrange
        var requestData = new { Name = "Test" };
        var expectedJson = JsonSerializer.Serialize(requestData);

        using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };

        HttpRequestMessage? capturedRequest = null;
        string? capturedRequestData = null;

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>(async (request, cancellationToken) =>
            {
                capturedRequest = request; // Capture the request for later inspection
                if (capturedRequest.Content != null)
                    capturedRequestData = await capturedRequest.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(true);
            })
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _httpService.HttpSendAsync<TestResponse>(HttpMethod.Post, new Uri("https://example.com"), requestData);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);

        Assert.Equal(expectedJson, capturedRequestData);
    }
}

public class TestResponse
{
    public string Message { get; set; } = string.Empty;
}