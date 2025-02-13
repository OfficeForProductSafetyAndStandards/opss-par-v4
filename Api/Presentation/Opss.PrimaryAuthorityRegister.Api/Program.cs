using Opss.PrimaryAuthorityRegister.Api.Application.Extensions;
using Opss.PrimaryAuthorityRegister.Api.Extensions;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Opss.PrimaryAuthorityRegister.Api;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddApplicationLayer();
        builder.Services.AddPersistenceLayer(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        // Add exception handler middleware
        app.UseExceptionHandler(config =>
        {
            config.AddExceptionHandlers(app.Environment.IsDevelopment());
        });

        app.Use(async (context, next) =>
        {
            var entityClaims = new Dictionary<string, string>();
            foreach (var header in context.Request.Headers)
            {
                if (header.Key.StartsWith("X-Entity-"))
                {
                    var entityKey = header.Key.Replace("X-Entity-", "");
                    entityClaims[entityKey] = header.Value;
                }
            }
            context.Items["EntityClaims"] = entityClaims;
            await next();
        });

        app.MapControllers();

        app.MigrateDatabase();

        await app.RunAsync().ConfigureAwait(false);
    }
}