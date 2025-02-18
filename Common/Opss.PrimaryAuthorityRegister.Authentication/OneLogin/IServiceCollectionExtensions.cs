using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opss.PrimaryAuthorityRegister.Authentication.Builders;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;

namespace Opss.PrimaryAuthorityRegister.Authentication.OneLogin;

public static class IServiceCollectionExtensions
{
    public static void AddOneLoginAuthentication(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var oneLoginAuthConfigSection = builder.Configuration.GetSection("OneLoginAuth");
        var section = oneLoginAuthConfigSection.Get<OpenIdConnectAuthConfig>();
        var oneLoginAuthConfig = section
            ?? throw new InvalidOperationException("Cannot load OneLogin auth configuration");

        var openIdConnectBuilder = new OpenIdConnectBuilder(oneLoginAuthConfig);
        builder.Services
            .AddTransient<OneLoginOpenIdConnectEvents>()
            .AddAuthentication(OpenIdConnectBuilder.ConfigureAuthentication)
            .AddCookie(openIdConnectBuilder.ConfigureCookie)
            .AddOpenIdConnect("oidc-onelogin", openIdConnectBuilder.ConfigureOneLoginOpenIdConnectOptions);
    }
}
