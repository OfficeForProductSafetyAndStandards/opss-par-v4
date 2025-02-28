using Microsoft.Extensions.DependencyInjection;
using Opss.PrimaryAuthorityRegister.Api.Application.Authentication;
using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceProviders;
using Opss.PrimaryAuthorityRegister.Authentication.TokenHandler;
using System.Reflection;

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
        services.AddAuthorisation();
        services.Addauthentication();
    }

    private static void Addauthentication(this IServiceCollection services)
    {
        services.AddTransient<IAuthenticatedUserService, OpenIdConnectUserService>();
        services.AddTransient<IJwtHandler, JwtHandler>();
        services.AddTransient<ITokenService, OpenIdConnectTokenService>();
        services.AddTransient<IUserClaimsService, UserClaimsService>();
    }

    private static void AddAuthorisation(this IServiceCollection services)
    {
        var assembly = typeof(IServiceCollectionExtensions).Assembly;
        services.AddAllTypesOf<IResourceKeyExpander>(assembly);

        services.AddTransient<IResourceClaimProvider, AttributeBasedResourceClaimProvider>();
        services.AddTransient<IClaimChecker, ClaimChecker>();
        services.AddHttpContextAccessor();

        // Add ClaimsPrincipal.
        services.AddScoped(UserClaimsServiceProvider.BuildClaims);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            cfg.AddOpenBehavior(typeof(ClaimAuthorisationBehaviour<,>));
        });
    }

    /// <summary>
    /// Register all implementations of a specific interface (e.g., IIdentityAppUserIdMapper)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    public static void AddAllTypesOf<T>(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var interfaceType = typeof(T);
        var implementations = assembly.GetTypes()
            .Where(type => interfaceType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract);

        foreach (var implementation in implementations)
        {
            services.AddTransient(interfaceType, implementation);
        }
    }
}
