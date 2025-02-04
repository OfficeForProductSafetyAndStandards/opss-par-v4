using System.Net;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Opss.PrimaryAuthorityRegister.Api.Extensions;
using Opss.PrimaryAuthorityRegister.Common.Problem;

namespace Opss.PrimaryAuthorityRegister.Api.UnitTests.Extensions;

public class IApplicationBuilderExtensionsTests
{
    private async Task<HttpResponseMessage> SendRequestWithException(TestServer server, Exception exception)
    {
        return await server.CreateClient().GetAsync($"?throw={exception?.GetType().Name}").ConfigureAwait(true);
    }

    private static readonly JsonSerializerOptions _serializationOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters =
                    {
                        new JsonStringEnumConverter()
                    }
    };

    [Fact]
    public async Task AddExceptionHandlers_ShouldReturnUnauthorized_WhenSecurityExceptionOccurs()
    {
        // Arrange
        using var server = new TestServer(new WebHostBuilder().Configure(app =>
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Query.ContainsKey("throw"))
                {
                    var feature = new ExceptionHandlerFeature { Error = new SecurityException("Unauthorized") };
                    context.Features.Set<IExceptionHandlerFeature>(feature);
                }
                await next().ConfigureAwait(false);
            });

            app.AddExceptionHandlers();
        }));

        // Act
        var response = await SendRequestWithException(server, new SecurityException());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, _serializationOptions);
        Assert.Equal(HttpStatusCode.Unauthorized, problemDetails?.Status);
    }

    [Fact]
    public async Task AddExceptionHandlers_ShouldReturnInternalServerError_WhenGenericExceptionOccurs()
    {
        // Arrange
        using var server = new TestServer(new WebHostBuilder().Configure(app =>
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Query.ContainsKey("throw"))
                {
                    var feature = new ExceptionHandlerFeature { Error = new Exception("Internal Error") };
                    context.Features.Set<IExceptionHandlerFeature>(feature);
                }
                await next().ConfigureAwait(false);
            });

            app.AddExceptionHandlers();
        }));

        // Act
        var response = await SendRequestWithException(server, new Exception());

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, _serializationOptions);
        Assert.Equal(HttpStatusCode.InternalServerError, problemDetails?.Status);
    }
}
