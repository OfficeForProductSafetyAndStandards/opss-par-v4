using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Application.Settings;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Common.Exceptions;
using IdentityConstants = Opss.PrimaryAuthorityRegister.Common.Constants.IdentityConstants;

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
                
        var localRoles = unitOfWork.Repository<Role>().Local();

        var userRepo = scope.ServiceProvider.GetRequiredService<IUserIdentityRepository>();
        foreach (var identity in seedData.Identities)
        {
            var existingUser = userRepo.GetUserIdentiyByEmail(identity.Email);
            if (existingUser != null) continue;

            var role = localRoles.Single(r => r.Name == identity.Role);
            var user = new UserIdentity(identity.Email, role);
            
            await unitOfWork.Repository<UserIdentity>().AddAsync(user).ConfigureAwait(false);
        }        

        return await unitOfWork.Save(CancellationToken.None).ConfigureAwait(false);
    }
}
