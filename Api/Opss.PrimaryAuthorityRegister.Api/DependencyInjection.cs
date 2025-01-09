namespace Opss.PrimaryAuthorityRegister.Api;

internal static class DependencyInjection
{
    /// <summary>
    /// Add MediatR to the services
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddApiMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
