using Microsoft.EntityFrameworkCore;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;
using Opss.PrimaryAuthorityRegister.Common.Providers;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.UnitTests.Contexts;

public class ApplicationDbContextTests
{
    private readonly DateTimeOverrideProvider _dateTimeProvider;

    public ApplicationDbContextTests()
    {
            _dateTimeProvider = new DateTimeOverrideProvider();
    }

    private static DbContextOptions<ApplicationDbContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void CanCreateDbContext_WithValidOptions()
    {
        // Arrange
        var options = GetInMemoryOptions();

        // Act
        using var context = new ApplicationDbContext(options, _dateTimeProvider);

        // Assert
        Assert.NotNull(context);
        Assert.IsAssignableFrom<ApplicationDbContext>(context);
    }

    [Fact]
    public void DbSet_TestData_ShouldBeAccessible()
    {
        // Arrange
        var options = GetInMemoryOptions();
        using var context = new ApplicationDbContext(options, _dateTimeProvider);

        // Act
        var dbSet = context.TestData;

        // Assert
        Assert.NotNull(dbSet);
        Assert.IsAssignableFrom<DbSet<TestData>>(dbSet);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistData()
    {
        // Arrange
        var options = GetInMemoryOptions();
        using var context = new ApplicationDbContext(options, _dateTimeProvider);

        var ownerId = Guid.NewGuid();
        var testData = new TestData(ownerId, "Data");
        await context.TestData.AddAsync(testData);

        // Act
        var result = await context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, result); // 1 change should be saved
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSupportCancellationToken()
    {
        // Arrange
        var options = GetInMemoryOptions();
        using var context = new ApplicationDbContext(options, _dateTimeProvider);
        using var tokenSource = new CancellationTokenSource();

        var cancellationToken = tokenSource.Token;

        var ownerId = Guid.NewGuid();
        var testData = new TestData(ownerId, "Data");
        await context.TestData.AddAsync(testData);

        // Act
        var result = await context.SaveChangesAsync(cancellationToken);

        // Assert
        Assert.Equal(1, result); // 1 change should be saved
    }
}