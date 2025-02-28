using Microsoft.EntityFrameworkCore;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;
using Opss.PrimaryAuthorityRegister.Common.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.UnitTests.Contexts.DbSetTests;

public class TestDataSetTests
{
    private readonly DateTimeOverrideProvider _dateTimeProvider;

    public TestDataSetTests()
    {
        _dateTimeProvider = new DateTimeOverrideProvider();
    }

    private static DbContextOptions<ApplicationDbContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }
}
