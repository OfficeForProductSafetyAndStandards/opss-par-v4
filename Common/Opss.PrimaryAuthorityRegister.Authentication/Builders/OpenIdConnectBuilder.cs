using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using OneLoginOpenIdConnectEvents = Opss.PrimaryAuthorityRegister.Authentication.OneLogin.OneLoginOpenIdConnectEvents;

namespace Opss.PrimaryAuthorityRegister.Authentication.Builders;

public class OpenIdConnectBuilder
{
    private readonly OpenIdConnectAuthConfig _oidcAuthConfig;

    public OpenIdConnectBuilder(OpenIdConnectAuthConfig oidcAuthConfig)
    {
        _oidcAuthConfig = oidcAuthConfig;
    }

    public static void ConfigureAuthentication(AuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
    }

    public void ConfigureCookie(CookieAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Cookie = new CookieBuilder
        {
            HttpOnly = true,
            Name = OpenIdConnectCookies.ParToken,
            Path = "/",
            SameSite = SameSiteMode.Strict,
            SecurePolicy = CookieSecurePolicy.Always,
            MaxAge = TimeSpan.FromMinutes(_oidcAuthConfig.CookieMaxAge),
        };
        options.SlidingExpiration = false;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(_oidcAuthConfig.CookieMaxAge);
    }

    public void ConfigureOneLoginOpenIdConnectOptions(OpenIdConnectOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Authority = _oidcAuthConfig.Authority;
        options.ClientId = _oidcAuthConfig.ClientId;

        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ResponseMode = OpenIdConnectResponseMode.Query;
        options.EventsType = typeof(OneLoginOpenIdConnectEvents);

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("email");

        options.MetadataAddress = $"{_oidcAuthConfig.Authority}/.well-known/openid-configuration";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = $"{_oidcAuthConfig.Authority}/",
            ValidAudience = _oidcAuthConfig.ClientId,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromSeconds(_oidcAuthConfig.ClockSkewSeconds),
        };
    }
}