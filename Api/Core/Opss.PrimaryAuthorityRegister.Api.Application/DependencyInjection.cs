using Microsoft.Extensions.DependencyInjection;

namespace Opss.PrimaryAuthorityRegister.Api.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
