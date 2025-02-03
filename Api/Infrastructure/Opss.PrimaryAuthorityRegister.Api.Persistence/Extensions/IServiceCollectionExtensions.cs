using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Behaviour;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Repositories;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration);
        services.AddRepositories();

        services.AddBehaviours();
    }

    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Configuration missing value for DefaultConnection connection string");

        services.AddDbContextPool<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, ConfigureSqlOptions)
        );
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services
            .AddTransient<IUnitOfWork, UnitOfWork>()
            .AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    }

    private static void ConfigureSqlOptions(NpgsqlDbContextOptionsBuilder sqlOptions)
    {
        sqlOptions.MigrationsAssembly(typeof(IServiceCollectionExtensions).Assembly.FullName);

        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
    }

    private static void AddBehaviours(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(IServiceCollectionExtensions).Assembly);

            cfg.AddOpenBehavior(typeof(PersistBehaviour<,>));
        });
    }
}
