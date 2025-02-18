using Opss.PrimaryAuthorityRegister.Api.Application.Extensions;
using Opss.PrimaryAuthorityRegister.Api.Extensions;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Extensions;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.OneLogin;
using Opss.PrimaryAuthorityRegister.Http.Services;
using System.Diagnostics.CodeAnalysis;

namespace Opss.PrimaryAuthorityRegister.Api;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var oneLoginAuthConfigSection = builder.Configuration.GetSection("OneLoginAuth");
        builder.Services.Configure<OpenIdConnectAuthConfig>(oneLoginAuthConfigSection);

        var jwtAuthConfigSection = builder.Configuration.GetSection("JwtAuth");
        builder.Services.Configure<JwtAuthConfig>(jwtAuthConfigSection);

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

        builder.AddOneLoginAuthentication();

        builder.Services.AddScoped<IHttpService, HttpService>();

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

        // Add exception handler middleware
        app.UseExceptionHandler(config =>
        {
            config.AddExceptionHandlers(app.Environment.IsDevelopment());
        });

        app.MapControllers();

        app.MigrateDatabase();

        await app.RunAsync().ConfigureAwait(false);
    }
}