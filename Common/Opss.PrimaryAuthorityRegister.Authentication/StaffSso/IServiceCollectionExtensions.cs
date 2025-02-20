using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opss.PrimaryAuthorityRegister.Authentication.Builders;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.OneLogin;
using Opss.PrimaryAuthorityRegister.Http.Services;

namespace Opss.PrimaryAuthorityRegister.Authentication.StaffSso;

public static class IServiceCollectionExtensions
{
    public static void AddStaffSsoAuthentication(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        var staffSsoAuthConfigSection = builder.Configuration.GetSection("StaffSSOAuth");
        var staffSsoSection = staffSsoAuthConfigSection.Get<OpenIdConnectAuthConfig>();
        var staffSsoAuthConfig = staffSsoSection
            ?? throw new InvalidOperationException("Cannot load StaffSSOAuth auth configuration");
        
        var jwtAuthConfigSection = builder.Configuration.GetSection("JwtAuth");
        var jwtSection = jwtAuthConfigSection.Get<JwtAuthConfig>();
        var jwtConfig = jwtSection
            ?? throw new InvalidOperationException("Cannot load JwtAuth auth configuration");
        
        var serviceProvider = builder.Services.BuildServiceProvider();
        var httpService = serviceProvider.GetRequiredService<IHttpService>();
        
        var openIdConnectBuilder = new OpenIdConnectBuilder(staffSsoAuthConfig, jwtConfig, httpService);
        builder.Services
            .AddTransient((IServiceProvider provider) => new OpssOpenIdConnectEvents(staffSsoAuthConfig, jwtConfig, httpService))
            .AddAuthentication(OpenIdConnectBuilder.ConfigureAuthentication)
            //.AddCookie(openIdConnectBuilder.ConfigureCookie)
            .AddOpenIdConnect("oidc-staffsso", openIdConnectBuilder.ConfigureOpenIdConnectOptions);
    }
}
