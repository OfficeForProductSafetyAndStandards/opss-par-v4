using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using System.Reflection;
using System.Security.Claims;

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
    }

    private static void AddAuthorisation(this IServiceCollection services)
    {
        var assembly = typeof(IServiceCollectionExtensions).Assembly;
        services.AddAllTypesOf<IResourceKeyExpander>(assembly);

        services.AddTransient<IResourceClaimProvider, AttributeBasedResourceClaimProvider>();
        services.AddTransient<IClaimChecker, ClaimChecker>();
        services.AddHttpContextAccessor();

        // Add ClaimsPrincipal.
        services.AddScoped(provider =>
        {
            return GenerateTempIdentity();
#pragma warning disable S125 // Sections of code should not be commented out
            // Code is commented out until authentication is implemented.
            // var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            // return httpContextAccessor.HttpContext?.User ?? GenerateTempIdentity();
#pragma warning restore S125 // Sections of code should not be commented out
        }
);

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

    private static ClaimsPrincipal GenerateTempIdentity()
    {
        var claimsIdentity = new ClaimsIdentity(
                                                Array.Empty<Claim>(),
                                                "Bearer",
                                                ClaimTypes.Name,
                                                ClaimTypes.Role
                                            );

        // Optionally add a name claim
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, "User"));
        claimsIdentity.AddClaim(new Claim(PermissionAttribute.PermissionClaimType, $"*", "Authority"));
        // give the user claims to that test data owner.
        claimsIdentity.AddClaim(new Claim(PermissionAttribute.PermissionClaimType, $"Owner/e3e695cc-ca85-43d8-9add-aa004eea5be5", "Create"));
        claimsIdentity.AddClaim(new Claim(PermissionAttribute.PermissionClaimType, $"Owner/e3e695cc-ca85-43d8-9add-aa004eea5be5", "Write"));
        claimsIdentity.AddClaim(new Claim(PermissionAttribute.PermissionClaimType, $"Owner/e3e695cc-ca85-43d8-9add-aa004eea5be5", "Read"));

        var principal = new ClaimsPrincipal(claimsIdentity);

        return principal;
    }
}
