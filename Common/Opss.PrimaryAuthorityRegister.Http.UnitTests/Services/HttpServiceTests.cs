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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


namespace Opss.PrimaryAuthorityRegister.Http.UnitTests.Services;

public class HttpServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly HttpService _httpService;
    private Mock<IHttpContextAccessor> _contextAccessor;

    public HttpServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _contextAccessor = new Mock<IHttpContextAccessor>();
        _httpService = new HttpService(_httpClient, _contextAccessor.Object);
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

        var contextAccessor = CreateMockHttpContextAccessor("A token");
        var httpService = new HttpService(_httpClient, contextAccessor.Object);

        // Act
        var result = await httpService.HttpSendAsync<TestResponse>(HttpMethod.Get, new Uri("https://example.com"));

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

        var contextAccessor = CreateMockHttpContextAccessor("A token");
        var httpService = new HttpService(_httpClient, contextAccessor.Object);

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
        var result = await httpService.HttpSendAsync<TestResponse>(HttpMethod.Post, new Uri("https://example.com"), requestData);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);

        Assert.Equal(expectedJson, capturedRequestData);
    }

    public static Mock<IHttpContextAccessor> CreateMockHttpContextAccessor(string tokenValue)
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var mockHttpContext = new Mock<HttpContext>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockAuthService = new Mock<IAuthenticationService>();

        // Create authentication properties and manually store the token in Items
        var authProperties = new AuthenticationProperties();
        authProperties.Items[".Token.par_token"] = tokenValue;  // Correct way to store tokens

        // Create an authentication ticket with these properties
        var authTicket = new AuthenticationTicket(
            new ClaimsPrincipal(new ClaimsIdentity()),
            authProperties,
            "Bearer");

        var authResult = AuthenticateResult.Success(authTicket);

        // Mock IAuthenticationService.AuthenticateAsync() to return the authentication result
        mockAuthService
            .Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(authResult);

        // Setup service provider to return the authentication service
        mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(mockAuthService.Object);

        // Setup HttpContext to return the service provider
        mockHttpContext
            .Setup(ctx => ctx.RequestServices)
            .Returns(mockServiceProvider.Object);

        // Setup IHttpContextAccessor to return the mocked HttpContext
        mockHttpContextAccessor
            .Setup(a => a.HttpContext)
            .Returns(mockHttpContext.Object);

        return mockHttpContextAccessor;
    }
}

public class TestResponse
{
    public string Message { get; set; } = string.Empty;
}