using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;
using Opss.PrimaryAuthorityRegister.Common.Constants;
using Opss.PrimaryAuthorityRegister.Cqrs.Services;
using Opss.PrimaryAuthorityRegister.Http.Services;
using Opss.PrimaryAuthorityRegister.Web.Application.Helpers;
using Opss.PrimaryAuthorityRegister.Web.Application.Services;
using System.Diagnostics.CodeAnalysis;

namespace Opss.PrimaryAuthorityRegister.Web;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var authConfigs = builder.Configuration.GetSection("OpenIdConnectAuth");
        builder.Services.Configure<OpenIdConnectAuthConfigurations>(authConfigs);

        var jwtAuthConfigSection = builder.Configuration.GetSection("JwtAuth");
        builder.Services.Configure<JwtAuthConfig>(jwtAuthConfigSection);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "_parAllowOrigins",
                policy =>
                {
                    policy.WithOrigins("https://oidc.integration.account.gov.uk");
                });
        });

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        builder.Services.AddLocalization(options => options.ResourcesPath = "LanguageResources");
        builder.Services.AddControllers();

        builder.Services.AddAuthentication();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddAuthorizationCore();

        builder.Services.AddHttpClient();
        builder.Services.AddScoped(sp =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://api:8080/")
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        });

        builder.Services.AddSingleton<IQueryHelper, QueryHelper>();
        builder.Services.AddScoped<ICookieService, CookieService>();
        builder.Services.AddScoped<ICqrsService, CqrsService>();
        builder.Services.AddScoped<IHttpService, HttpService>();
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.AddOidcAuthentication("OneLogin");
        builder.AddOidcAuthentication("StaffSSO");

        builder.Services.AddAuthorization(config =>
        {
            foreach (var policy in IdentityConstants.Policies.Configuration)
                config.AddPolicy(policy.Key, p => p.RequireRole(policy.Value));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.MapControllers();

        app.UseCors("_parAllowOrigins");

        UseLocalization(app);

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        await app.RunAsync().ConfigureAwait(false);
    }

    private static void UseLocalization(WebApplication app)
    {
        var supportedCultures = new[] { "en", "cy" };
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);

    }
}