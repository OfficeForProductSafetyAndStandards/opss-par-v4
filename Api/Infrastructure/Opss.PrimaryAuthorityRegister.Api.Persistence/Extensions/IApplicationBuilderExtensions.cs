using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Extensions;

public static class IApplicationBuilderExtensions
{
    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
    }
}
