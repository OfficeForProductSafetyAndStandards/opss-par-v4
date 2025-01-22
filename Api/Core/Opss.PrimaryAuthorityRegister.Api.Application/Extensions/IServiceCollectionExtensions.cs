using Microsoft.Extensions.DependencyInjection;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Extensions;

/// <summary>
/// Extension methods for the <c>IServiceCollection</c> to add all services required for the 
/// application layer.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds all services required for the application layer.
    /// </summary>
    /// <param name="services">The IServiceCollection to add application services to.</param>
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediator();
    }

    private static void AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(IServiceCollectionExtensions).Assembly));
    }
}
