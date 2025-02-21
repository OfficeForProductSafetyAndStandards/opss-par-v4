using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Opss.PrimaryAuthorityRegister.Authentication.Builders;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Http.Services;

namespace Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;

public static class IServiceCollectionExtensions
{
    public static void AddOidcAuthentication(this WebApplicationBuilder builder, string providerKey)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerKey);

        if (!builder.Configuration.TryGetSection<OpenIdConnectAuthConfigurations>("OpenIdConnectAuth", out var authConfig))
            throw new InvalidOperationException($"Cannot load {providerKey} auth configuration");
        var providerAuthConfig = authConfig!.Providers[providerKey];

        if (!builder.Configuration.TryGetSection<JwtAuthConfig>("JwtAuth", out var jwtConfig))
            throw new InvalidOperationException("Cannot load JwtAuth auth configuration");

        var serviceProvider = builder.Services.BuildServiceProvider();
        var cqrsService = serviceProvider.GetRequiredService<ICqrsService>();
        
        builder.Services
            .AddTransient((provider) => new OpssOpenIdConnectEvents(providerAuthConfig, jwtConfig!, cqrsService));

        var openIdConnectBuilder = new OpenIdConnectBuilder(providerAuthConfig, jwtConfig!, cqrsService);
        var authBuilder = builder.Services.AddAuthentication(OpenIdConnectBuilder.ConfigureAuthentication);

        // We only want to register the auth cookies once, rather than once per auth provider.
        bool cookieConfigured = authBuilder
            .Services
            .Any(sd =>
                sd.ServiceType == typeof(IPostConfigureOptions<CookieAuthenticationOptions>));
        if (!cookieConfigured)
        {
            authBuilder.AddCookie(openIdConnectBuilder.ConfigureCookie);
        }

        authBuilder.AddOpenIdConnect(
            "oidc-" + providerKey.ToLowerInvariant(), 
            openIdConnectBuilder.ConfigureOpenIdConnectOptions);
    }
}
