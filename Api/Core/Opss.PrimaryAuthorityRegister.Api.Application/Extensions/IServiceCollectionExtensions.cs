using Microsoft.Extensions.DependencyInjection;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediator();
    }

    private static void AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(IServiceCollectionExtensions).Assembly));
    }
}
