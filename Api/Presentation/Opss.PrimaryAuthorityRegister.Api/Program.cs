using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Opss.PrimaryAuthorityRegister.Api.Application;
using Opss.PrimaryAuthorityRegister.Api.Application.Authentication;
using Opss.PrimaryAuthorityRegister.Api.Application.Extensions;
using Opss.PrimaryAuthorityRegister.Api.Application.Services;
using Opss.PrimaryAuthorityRegister.Api.Application.Settings;
using Opss.PrimaryAuthorityRegister.Api.Extensions;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Extensions;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.Jwt;
using Opss.PrimaryAuthorityRegister.Authentication.Middleware;
using Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Common.Providers;
using Opss.PrimaryAuthorityRegister.Http.Services;
using System.Diagnostics.CodeAnalysis;

namespace Opss.PrimaryAuthorityRegister.Api;

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

        var seedDataSection = builder.Configuration.GetSection("SeedData");
        builder.Services.Configure<SeedData>(seedDataSection);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddApplicationLayer();
        builder.Services.AddPersistenceLayer(builder.Configuration);

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

        builder.Services.AddScoped<IUserRoleService, UserRoleService>();
        builder.Services.AddScoped<ICqrsService, CqrsService>();
        builder.Services.AddScoped<IHttpService, HttpService>();
        builder.Services.AddScoped<IUserClaimsService, UserClaimsService>();
        builder.Services.AddScoped<IJwtService, JwtService>();

        builder.Services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        builder.AddOidcAuthentication("OneLogin");
        builder.AddOidcAuthentication("StaffSSO");

        builder.Services.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>().Get("Bearer");
            return options.TokenValidationParameters;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<TokenToClaimsMiddleware>();

        // Add exception handler middleware
        app.UseExceptionHandler(config =>
        {
            config.AddExceptionHandlers(app.Environment.IsDevelopment());
        });

        app.MapControllers();

        app.MigrateDatabase();
        await app.SeedIdentity().ConfigureAwait(false);

        await app.RunAsync().ConfigureAwait(false);
    }
}