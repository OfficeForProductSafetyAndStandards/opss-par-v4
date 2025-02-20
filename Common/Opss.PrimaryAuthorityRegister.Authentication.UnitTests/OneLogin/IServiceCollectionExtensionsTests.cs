using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;
using Opss.PrimaryAuthorityRegister.Http.Services;
namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.OneLogin;

public class IServiceCollectionExtensionsTests
{
    [Fact]
    public void AddAuthentication_ShouldThrowException_WhenBuilderIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => IServiceCollectionExtensions.AddOidcAuthentication(null!, "OneLogin"));
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
        var exception = Assert.Throws<InvalidOperationException>(() => builder.AddOidcAuthentication("OneLogin"));
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
            {"OneLoginAuth:Authority" ,"https://example.com" },
            {"JwtAuth:SecurityKey", "test-key"},
            {"JwtAuth:MinutesUntilExpiration", "30"},
            {"JwtAuth:Issuer", "https://example.com"},
            {"JwtAuth:ClockSkewSeconds", "30"},
            { "JwtAuth:Audience", "https://localhost/" },
        };

        var config = new ConfigurationBuilder()
           .AddInMemoryCollection(authConfig)
           .Build();

        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddConfiguration(config);
        builder.Services.AddHttpClient();
        builder.Services.AddScoped((IServiceProvider provider) => new Mock<IHttpService>().Object);

        // Act
        builder.AddOidcAuthentication("OneLogin");

        // Assert
        var services = builder.Services;
        Assert.Contains(services, s => s.ServiceType.FullName == typeof(OpssOpenIdConnectEvents).FullName);
    }
}

