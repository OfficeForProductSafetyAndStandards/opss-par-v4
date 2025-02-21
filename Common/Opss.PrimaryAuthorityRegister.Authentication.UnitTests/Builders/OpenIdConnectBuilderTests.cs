using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.Builders;
using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using Opss.PrimaryAuthorityRegister.Http.ExtensionMethods;
using Opss.PrimaryAuthorityRegister.Http.Services;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.Builders;

public class OpenIdConnectBuilderTests
{
    private readonly OpenIdConnectBuilder _builder;

    public OpenIdConnectBuilderTests()
    {
        _builder = new OpenIdConnectBuilder(
            AuthenticationTestHelpers.ProviderAuthConfig, 
            AuthenticationTestHelpers.JwtConfig, 
            new Mock<ICqrsService>().Object);
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
        Assert.Equal(TimeSpan.FromMinutes(AuthenticationTestHelpers.ProviderAuthConfig.CookieMaxAge), options.Cookie.MaxAge);
        Assert.False(options.SlidingExpiration);
        Assert.Equal(TimeSpan.FromMinutes(AuthenticationTestHelpers.ProviderAuthConfig.CookieMaxAge), options.ExpireTimeSpan);
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
        _builder.ConfigureOpenIdConnectOptions(options);

        // Assert
        Assert.Equal(AuthenticationTestHelpers.ProviderAuthConfig.AuthorityUri.ToString(), options.Authority);
        Assert.Equal(AuthenticationTestHelpers.ProviderAuthConfig.ClientId, options.ClientId);
        Assert.Equal(OpenIdConnectResponseType.Code, options.ResponseType);
        Assert.Equal(OpenIdConnectResponseMode.Query, options.ResponseMode);
        Assert.NotNull(options.Events);
        Assert.True(options.SaveTokens);
        Assert.True(options.GetClaimsFromUserInfoEndpoint);
        Assert.Contains("openid", options.Scope);
        Assert.Contains("email", options.Scope);
        Assert.Equal(AuthenticationTestHelpers.ProviderAuthConfig.AuthorityUri.AppendPath(".well-known/openid-configuration").ToString(), options.MetadataAddress);
        Assert.Equal(AuthenticationTestHelpers.ProviderAuthConfig.IssuerUri.ToString(), options.TokenValidationParameters.ValidIssuer);
        Assert.Equal(AuthenticationTestHelpers.ProviderAuthConfig.ClientId, options.TokenValidationParameters.ValidAudience);
        Assert.True(options.TokenValidationParameters.ValidateIssuerSigningKey);
        Assert.Equal(TimeSpan.FromSeconds(AuthenticationTestHelpers.ProviderAuthConfig.ClockSkewSeconds), options.TokenValidationParameters.ClockSkew);
    }

    [Fact]
    public void ConfigureOneLoginOpenIdConnectOptions_ShouldThrowException_WhenOptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _builder.ConfigureOpenIdConnectOptions(null));
    }
}
