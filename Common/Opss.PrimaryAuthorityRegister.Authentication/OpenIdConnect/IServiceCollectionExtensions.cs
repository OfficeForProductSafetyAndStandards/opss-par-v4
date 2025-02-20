using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
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

        var openIdConnectAuthConfigSections = builder.Configuration.GetSection("OpenIdConnectAuth");
        var oneLoginSection = openIdConnectAuthConfigSections.Get<OpenIdConnectAuthConfigurations>();
        var oneLoginAuthConfig = oneLoginSection
            ?? throw new InvalidOperationException("Cannot load auth configuration");

        var oneLoginConfig = oneLoginAuthConfig.Providers[providerKey];

        var jwtAuthConfigSection = builder.Configuration.GetSection("JwtAuth");
        var jwtSection = jwtAuthConfigSection.Get<JwtAuthConfig>();
        var jwtConfig = jwtSection
            ?? throw new InvalidOperationException("Cannot load JwtAuth auth configuration");

        var serviceProvider = builder.Services.BuildServiceProvider();
        var httpService = serviceProvider.GetRequiredService<IHttpService>();

        var openIdConnectBuilder = new OpenIdConnectBuilder(oneLoginConfig, jwtConfig, httpService);
        builder.Services
            .AddTransient((provider) => new OpssOpenIdConnectEvents(oneLoginConfig, jwtConfig, httpService));

        var authBuilder = builder.Services.AddAuthentication(OpenIdConnectBuilder.ConfigureAuthentication);
        bool cookieConfigured = authBuilder.Services.Any(sd => sd.ServiceType == typeof(IPostConfigureOptions<CookieAuthenticationOptions>));
        if (!cookieConfigured)
        {
            authBuilder.AddCookie(openIdConnectBuilder.ConfigureCookie);
        }
        authBuilder.AddOpenIdConnect("oidc-" + providerKey.ToLower(), openIdConnectBuilder.ConfigureOpenIdConnectOptions);
    }
}
