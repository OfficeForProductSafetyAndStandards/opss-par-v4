using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.OneLogin;
using System.Text.Json;
namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.OneLogin;

public class IServiceCollectionExtensionsTests
{
    private readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    [Fact]
    public void AddAuthentication_ShouldThrowException_WhenBuilderIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => IServiceCollectionExtensions.AddOneLoginAuthentication(null!));
    }

    [Fact]
    public void AddAuthentication_ShouldThrowException_WhenAuthConfigIsMissing()
    {
        // Arrange
        var authConfig = new Dictionary<string, string>();

        var config = new ConfigurationBuilder()
           .AddInMemoryCollection(authConfig)
           .Build();

        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddConfiguration(config);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.AddOneLoginAuthentication());
        Assert.Equal("Cannot load OneLogin auth configuration", exception.Message);
    }

    [Fact]
    public void AddAuthentication_ShouldConfigureServicesCorrectly()
    {
        // Arrange
        var authConfig = new Dictionary<string, string>
        {
            {"OneLoginAuth:RsaPrivateKey" , "RSA Key" },
            {"OneLoginAuth:PostLogoutRedirectUri" , "http://localhost/"},
            {"OneLoginAuth:CookieMaxAge" , "30"},
            {"OneLoginAuth:ClockSkewSeconds" , "60"},
            {"OneLoginAuth:ClientId" , "test-client-id"},
            { "OneLoginAuth:Authority" ,"https://example.com" }
        };

        var config = new ConfigurationBuilder()
           .AddInMemoryCollection(authConfig)
           .Build();

        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddConfiguration(config);

        // Act
        builder.AddOneLoginAuthentication();

        // Assert
        var services = builder.Services;
        Assert.Contains(services, s => s.ServiceType.FullName == typeof(OneLoginOpenIdConnectEvents).FullName);
    }
}

