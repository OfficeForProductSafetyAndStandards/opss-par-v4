using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Opss.PrimaryAuthorityRegister.Authentication.Builders;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using Opss.PrimaryAuthorityRegister.Authentication.OneLogin;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.Builders;

public class OpenIdConnectBuilderTests
{
    private readonly OpenIdConnectAuthConfig _config;
    private readonly OpenIdConnectBuilder _builder;

    public OpenIdConnectBuilderTests()
    {
        _config = new OpenIdConnectAuthConfig
        {
            Authority = "https://example.com",
            ClientId = "client-id",
            CookieMaxAge = 60,
            ClockSkewSeconds = 300,
            PostLogoutRedirectUri =  new Uri("https://localhost/"),
            RsaPrivateKey = "RSAKey"
        };
        _builder = new OpenIdConnectBuilder(_config);
    }

    [Fact]
    public void ConfigureAuthentication_ShouldSetCorrectSchemes()
    {
        // Arrange
        var options = new AuthenticationOptions();

        // Act
        OpenIdConnectBuilder.ConfigureAuthentication(options);

        // Assert
        Assert.Equal(CookieAuthenticationDefaults.AuthenticationScheme, options.DefaultAuthenticateScheme);
        Assert.Equal(CookieAuthenticationDefaults.AuthenticationScheme, options.DefaultSignInScheme);
        Assert.Equal(OpenIdConnectDefaults.AuthenticationScheme, options.DefaultChallengeScheme);
        Assert.Equal(OpenIdConnectDefaults.AuthenticationScheme, options.DefaultSignOutScheme);
    }

    [Fact]
    public void ConfigureAuthentication_ShouldThrowException_WhenOptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => OpenIdConnectBuilder.ConfigureAuthentication(null));
    }

    [Fact]
    public void ConfigureCookie_ShouldSetCorrectCookieOptions()
    {
        // Arrange
        var options = new CookieAuthenticationOptions();

        // Act
        _builder.ConfigureCookie(options);

        // Assert
        Assert.Equal(OpenIdConnectCookies.ParToken, options.Cookie.Name);
        Assert.Equal("/", options.Cookie.Path);
        Assert.Equal(SameSiteMode.Strict, options.Cookie.SameSite);
        Assert.Equal(CookieSecurePolicy.Always, options.Cookie.SecurePolicy);
        Assert.Equal(TimeSpan.FromMinutes(_config.CookieMaxAge), options.Cookie.MaxAge);
        Assert.False(options.SlidingExpiration);
        Assert.Equal(TimeSpan.FromMinutes(_config.CookieMaxAge), options.ExpireTimeSpan);
    }

    [Fact]
    public void ConfigureCookie_ShouldThrowException_WhenOptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _builder.ConfigureCookie(null));
    }

    [Fact]
    public void ConfigureOneLoginOpenIdConnectOptions_ShouldSetCorrectOptions()
    {
        // Arrange
        var options = new OpenIdConnectOptions();

        // Act
        _builder.ConfigureOneLoginOpenIdConnectOptions(options);

        // Assert
        Assert.Equal(_config.Authority, options.Authority);
        Assert.Equal(_config.ClientId, options.ClientId);
        Assert.Equal(OpenIdConnectResponseType.Code, options.ResponseType);
        Assert.Equal(OpenIdConnectResponseMode.Query, options.ResponseMode);
        Assert.Equal(typeof(OneLoginOpenIdConnectEvents), options.EventsType);
        Assert.True(options.SaveTokens);
        Assert.True(options.GetClaimsFromUserInfoEndpoint);
        Assert.Contains("openid", options.Scope);
        Assert.Contains("email", options.Scope);
        Assert.Equal($"{_config.Authority}/.well-known/openid-configuration", options.MetadataAddress);
        Assert.Equal($"{_config.Authority}/", options.TokenValidationParameters.ValidIssuer);
        Assert.Equal(_config.ClientId, options.TokenValidationParameters.ValidAudience);
        Assert.True(options.TokenValidationParameters.ValidateIssuerSigningKey);
        Assert.Equal(TimeSpan.FromSeconds(_config.ClockSkewSeconds), options.TokenValidationParameters.ClockSkew);
    }

    [Fact]
    public void ConfigureOneLoginOpenIdConnectOptions_ShouldThrowException_WhenOptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _builder.ConfigureOneLoginOpenIdConnectOptions(null));
    }
}
