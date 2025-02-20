using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opss.PrimaryAuthorityRegister.Authentication.Builders;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Http.Services;

namespace Opss.PrimaryAuthorityRegister.Authentication.OneLogin;

public static class IServiceCollectionExtensions
{
    public static void AddOneLoginAuthentication(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var oneLoginAuthConfigSection = builder.Configuration.GetSection("OneLoginAuth");
        var oneLoginSection = oneLoginAuthConfigSection.Get<OpenIdConnectAuthConfig>();
        var oneLoginAuthConfig = oneLoginSection
            ?? throw new InvalidOperationException("Cannot load OneLogin auth configuration");


        var jwtAuthConfigSection = builder.Configuration.GetSection("JwtAuth");
        var jwtSection = jwtAuthConfigSection.Get<JwtAuthConfig>();
        var jwtConfig = jwtSection
            ?? throw new InvalidOperationException("Cannot load JwtAuth auth configuration");

        var serviceProvider = builder.Services.BuildServiceProvider();
        var httpService = serviceProvider.GetRequiredService<IHttpService>();

        var openIdConnectBuilder = new OpenIdConnectBuilder(oneLoginAuthConfig, jwtConfig, httpService);
        builder.Services
            .AddTransient((IServiceProvider provider) => new OpssOpenIdConnectEvents(oneLoginAuthConfig, jwtConfig, httpService))
            .AddAuthentication(OpenIdConnectBuilder.ConfigureAuthentication)
            .AddCookie(openIdConnectBuilder.ConfigureCookie)
            .AddOpenIdConnect("oidc-onelogin", openIdConnectBuilder.ConfigureOpenIdConnectOptions);
    }
}
