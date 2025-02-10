using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Exceptions;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Extensions;

public static class IApplicationBuilderExtensions
{
    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var iServiceScopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
        if(iServiceScopeFactory == null)
        {
            throw new ServiceNotFoundException(nameof(IServiceScopeFactory));
        }

        using var scope = iServiceScopeFactory.CreateScope();

        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
    }
}
