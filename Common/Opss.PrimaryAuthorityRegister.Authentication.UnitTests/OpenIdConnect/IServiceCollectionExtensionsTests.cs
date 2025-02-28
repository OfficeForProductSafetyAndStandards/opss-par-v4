using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;
using Opss.PrimaryAuthorityRegister.Cqrs.Services;
using Opss.PrimaryAuthorityRegister.Http.Services;
namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.OpenIdConnect;

public class IServiceCollectionExtensionsTests
{
    [Fact]
    public void AddAuthentication_ShouldThrowException_WhenBuilderIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => IServiceCollectionExtensions.AddOidcAuthentication(null!, AuthenticationTestHelpers.AuthProviderKey));
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
        var exception = Assert.Throws<InvalidOperationException>(() => builder.AddOidcAuthentication(AuthenticationTestHelpers.AuthProviderKey));
        Assert.Equal($"Cannot load {AuthenticationTestHelpers.AuthProviderKey} auth configuration", exception.Message);
    }

    [Fact]
    public void AddAuthentication_ShouldConfigureServicesCorrectly()
    {
        // Arrange
        var authConfig = new Dictionary<string, string>
        {
            {$"OpenIdConnectAuth:Providers:{AuthenticationTestHelpers.AuthProviderKey}:ProviderKey", AuthenticationTestHelpers.ProviderAuthConfig.ProviderKey },
            {$"OpenIdConnectAuth:Providers:{AuthenticationTestHelpers.AuthProviderKey}:WellKnownPath", AuthenticationTestHelpers.ProviderAuthConfig.WellKnownPath },
            {$"OpenIdConnectAuth:Providers:{AuthenticationTestHelpers.AuthProviderKey}:UserInfoPath", AuthenticationTestHelpers.ProviderAuthConfig.UserInfoPath },
            {$"OpenIdConnectAuth:Providers:{AuthenticationTestHelpers.AuthProviderKey}:PostLogoutRedirectUri", AuthenticationTestHelpers.ProviderAuthConfig.PostLogoutRedirectUri.ToString() },
            {$"OpenIdConnectAuth:Providers:{AuthenticationTestHelpers.AuthProviderKey}:IssuerUri", AuthenticationTestHelpers.ProviderAuthConfig.IssuerUri.ToString() },
            {$"OpenIdConnectAuth:Providers:{AuthenticationTestHelpers.AuthProviderKey}:CookieMaxAge", AuthenticationTestHelpers.ProviderAuthConfig.CookieMaxAge.ToString() },
            {$"OpenIdConnectAuth:Providers:{AuthenticationTestHelpers.AuthProviderKey}:ClockSkewSeconds", AuthenticationTestHelpers.ProviderAuthConfig.ClockSkewSeconds.ToString() },
            {$"OpenIdConnectAuth:Providers:{AuthenticationTestHelpers.AuthProviderKey}:ClientSecret", AuthenticationTestHelpers.ProviderAuthConfig.ClientSecret },
            {$"OpenIdConnectAuth:Providers:{AuthenticationTestHelpers.AuthProviderKey}:ClientId", AuthenticationTestHelpers.ProviderAuthConfig.ClientId },
            {$"OpenIdConnectAuth:Providers:{AuthenticationTestHelpers.AuthProviderKey}:CallbackPath", AuthenticationTestHelpers.ProviderAuthConfig.CallbackPath },
            {$"OpenIdConnectAuth:Providers:{AuthenticationTestHelpers.AuthProviderKey}:AuthorityUri", AuthenticationTestHelpers.ProviderAuthConfig.AuthorityUri.ToString() },
            {$"OpenIdConnectAuth:Providers:{AuthenticationTestHelpers.AuthProviderKey}:AccessTokenPath", AuthenticationTestHelpers.ProviderAuthConfig.AccessTokenPath },
            {"JwtAuth:SecurityKey", "test-key"},
            {"JwtAuth:MinutesUntilExpiration", "30"},
            {"JwtAuth:Issuer", "https://example.com"},
            {"JwtAuth:ClockSkewSeconds", "30"},
            {"JwtAuth:Audience", "https://localhost/" },
        };

        var config = new ConfigurationBuilder()
           .AddInMemoryCollection(authConfig)
           .Build();

        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddConfiguration(config);
        builder.Services.AddHttpClient();
        builder.Services.AddScoped((provider) => new Mock<ICqrsService>().Object);
        builder.Services.AddScoped((provider) => new Mock<IHttpService>().Object);

        // Act
        builder.AddOidcAuthentication(AuthenticationTestHelpers.AuthProviderKey);

        // Assert
        var services = builder.Services;
        Assert.Contains(services, s => s.ServiceType.FullName == typeof(OpssOpenIdConnectEvents).FullName);
    }
}

