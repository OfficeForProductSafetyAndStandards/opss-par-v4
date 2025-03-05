using Microsoft.EntityFrameworkCore;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.UnitTests.Repositories;

internal static class InMemoryDatabaseTestHelpers
{
    internal static DbContextOptions<ApplicationDbContext> InMemoryOptions => new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
}