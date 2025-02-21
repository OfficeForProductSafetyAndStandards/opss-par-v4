using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;
using Opss.PrimaryAuthorityRegister.Http.ExtensionMethods;
using Opss.PrimaryAuthorityRegister.Http.Services;

namespace Opss.PrimaryAuthorityRegister.Authentication.Builders;

public class OpenIdConnectBuilder
{
    private readonly OpenIdConnectAuthConfiguration _oidcAuthConfig;
    private readonly JwtAuthConfig _jwtAuthConfig;
    private readonly ICqrsService _cqrsService;

    public OpenIdConnectBuilder(OpenIdConnectAuthConfiguration oidcAuthConfig, JwtAuthConfig jwtAuthConfig, ICqrsService cqrsService)
    {
        _oidcAuthConfig = oidcAuthConfig;
        _jwtAuthConfig = jwtAuthConfig;
        _cqrsService = cqrsService;
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

    public void ConfigureOpenIdConnectOptions(OpenIdConnectOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Authority = _oidcAuthConfig.AuthorityUri.ToString();
        options.ClientId = _oidcAuthConfig.ClientId;

        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ResponseMode = OpenIdConnectResponseMode.Query;
        options.Events = new OpssOpenIdConnectEvents(_oidcAuthConfig, _jwtAuthConfig, _cqrsService);

        options.CallbackPath = _oidcAuthConfig.CallbackPath;

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        if (!string.IsNullOrEmpty(_oidcAuthConfig.ClientSecret))
        {
            options.ClientSecret = _oidcAuthConfig.ClientSecret;
        }

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("email");

        options.MetadataAddress = _oidcAuthConfig.AuthorityUri.AppendPath(_oidcAuthConfig.WellKnownPath).ToString();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = _oidcAuthConfig.IssuerUri.ToString(),
            ValidAudience = _oidcAuthConfig.ClientId,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromSeconds(_oidcAuthConfig.ClockSkewSeconds),
        };
    }
}