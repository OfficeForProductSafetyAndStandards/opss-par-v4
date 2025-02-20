using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.OneLogin;
using Opss.PrimaryAuthorityRegister.Authentication.StaffSso;
using Opss.PrimaryAuthorityRegister.Http.Services;
using Opss.PrimaryAuthorityRegister.Web.Application.Services;
using System.Diagnostics.CodeAnalysis;

namespace Opss.PrimaryAuthorityRegister.Web;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var oneLoginAuthConfigSection = builder.Configuration.GetSection("OneLoginAuth");
        builder.Services.Configure<OpenIdConnectAuthConfig>(oneLoginAuthConfigSection);

        var staffSsoAuthConfigSection = builder.Configuration.GetSection("StaffSSOAuth");
        builder.Services.Configure<OpenIdConnectAuthConfig>(staffSsoAuthConfigSection);

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

        builder.Services.AddScoped<ICookieService, CookieService>();
        builder.Services.AddScoped<IHttpService, HttpService>();
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.AddOneLoginAuthentication();
        builder.AddStaffSsoAuthentication();

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