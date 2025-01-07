namespace Opss.PrimaryAuthorityRegister.Core.Api;

internal static class DependencyInjection
{
    public static IServiceCollection AddApiMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
