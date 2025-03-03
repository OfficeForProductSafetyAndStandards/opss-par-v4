using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Application.Settings;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Common.Constants;
using Opss.PrimaryAuthorityRegister.Common.Exceptions;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<int> SeedIdentity(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var iServiceScopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
        if (iServiceScopeFactory == null)
        {
            throw new ServiceNotFoundException(nameof(IServiceScopeFactory));
        }

        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

        var seedDataConfig = configuration.GetSection("SeedData");
        var seedData = seedDataConfig.Get<SeedData>();

        if (seedData is null) return 0;

        using var scope = iServiceScopeFactory.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var existingRoles = unitOfWork.Repository<Role>().Entities;
        
        foreach(var roleName in IdentityConstants.Roles.AllRoles)
        {
            if(existingRoles.SingleOrDefault(r => r.Name == roleName) != null)
                continue;

            await unitOfWork.Repository<Role>().AddAsync(new Role(roleName)).ConfigureAwait(false);
        }

        var userRepo = scope.ServiceProvider.GetRequiredService<IUserIdentityRepository>();
        // Only start seeding identities, if there are any to seed.
        if (seedData.Identities.Count > 0)
        {
            var localRoles = unitOfWork.Repository<Role>().Local();

            foreach (var identity in seedData.Identities)
            {
                var existingUser = userRepo.GetUserIdentiyByEmail(identity.Email);
                if (existingUser != null) continue;

                var role = localRoles.Single(r => r.Name == identity.Role);
                var user = new UserIdentity(identity.Email, role);

                await unitOfWork.Repository<UserIdentity>().AddAsync(user).ConfigureAwait(false);
            }
        }

        // Only bother getting regulatory functions if we're going to seed anything
        if (seedData.RegulatoryFunctions.Count > 0)
        {
            var existingRegulatoryFunctions = unitOfWork.Repository<RegulatoryFunction>().Entities;
            foreach (var functionName in seedData.RegulatoryFunctions)
            {
                var existingFunction = existingRegulatoryFunctions.SingleOrDefault(f => f.Name == functionName);
                if (existingFunction != null) continue;

                var newFunction = new RegulatoryFunction(functionName);

                await unitOfWork.Repository<RegulatoryFunction>().AddAsync(newFunction).ConfigureAwait(false);
            }
        }

        // Only seed Authorities if some are provided
        if(seedData.Authorities.Count > 0)
        {
            var existingRegulatoryFunctions = unitOfWork.Repository<RegulatoryFunction>().Local();
            var existingAuthorities = unitOfWork.Repository<Authority>().Entities;
            foreach(var seedAuthority in seedData.Authorities)
            {
                var existingAuthority = existingAuthorities.SingleOrDefault(a => a.Name == seedAuthority.Name);
                if (existingAuthority != null) continue;

                var newAuthority = new Authority(seedAuthority.Name);

                foreach(var identity in seedAuthority.Identities)
                {
                    var user = userRepo.GetUserIdentiyByEmail(identity);
                    if (user == null) continue;
                    newAuthority.AddUser(user);
                }

                foreach(var regFunction in seedAuthority.RegulatoryFunctions)
                {
                    var function = existingRegulatoryFunctions.SingleOrDefault(r => r.Name == regFunction);
                    if (function == null) continue;
                    newAuthority.AddRegulatoryFunction(function);
                }

                await unitOfWork.Repository<Authority>().AddAsync(newAuthority).ConfigureAwait(false);
            }
        }

        return await unitOfWork.Save(CancellationToken.None).ConfigureAwait(false);
    }
}
