using Microsoft.EntityFrameworkCore;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.UnitTests.Contexts;

public class ApplicationDbContextTests
{
    private static DbContextOptions<ApplicationDbContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique database name for isolation
            .Options;
    }

    [Fact]
    public void CanCreateDbContext_WithValidOptions()
    {
        // Arrange
        var options = GetInMemoryOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context);
        Assert.IsAssignableFrom<ApplicationDbContext>(context);
    }

    [Fact]
    public void DbSet_TestData_ShouldBeAccessible()
    {
        // Arrange
        var options = GetInMemoryOptions();
        using var context = new ApplicationDbContext(options);

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
        using var context = new ApplicationDbContext(options);

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
        using var context = new ApplicationDbContext(options);
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