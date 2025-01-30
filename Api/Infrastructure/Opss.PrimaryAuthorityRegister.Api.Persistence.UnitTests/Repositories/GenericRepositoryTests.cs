using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Repositories;

public class GenericRepositoryTests
{
    [Fact]
    public async Task Entities_ShouldReturnDataset()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        var repository = new GenericRepository<TestData>(context);

        var ownerId = Guid.NewGuid();
        var entity = new TestData(ownerId, "User1");
        await context.Set<TestData>().AddAsync(entity);
        await context.SaveChangesAsync();

        // Act
        var result = repository.Entities.SingleOrDefault();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result?.Id);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityToDbContext()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        var repository = new GenericRepository<TestData>(context);

        var ownerId = Guid.NewGuid();
        var entity = new TestData(ownerId, "Test");

        // Act
        var result = await repository.AddAsync(entity);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(entity, result);
        Assert.Contains(entity, context.Set<TestData>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntityFromDbContext()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        var repository = new GenericRepository<TestData>(context);

        var ownerId = Guid.NewGuid();
        var entity = new TestData(ownerId, "User1");
        await context.Set<TestData>().AddAsync(entity);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(entity);
        await context.SaveChangesAsync();

        // Assert
        Assert.DoesNotContain(entity, context.Set<TestData>());
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldRemoveEntityFromDbContextById()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        var repository = new GenericRepository<TestData>(context);

        var ownerId = Guid.NewGuid();
        var entity = new TestData(ownerId, "User1");
        await context.Set<TestData>().AddAsync(entity);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteByIdAsync(entity.Id);
        await context.SaveChangesAsync();

        // Assert
        Assert.DoesNotContain(entity, context.Set<TestData>());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntityById()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        var repository = new GenericRepository<TestData>(context);

        var ownerId = Guid.NewGuid();
        var entity = new TestData(ownerId, "User1");
        await context.Set<TestData>().AddAsync(entity);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(entity.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result?.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntityInDbContext()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        var repository = new GenericRepository<TestData>(context);

        var ownerId = Guid.NewGuid();
        var entity = new TestData(ownerId, "User1");
        await context.Set<TestData>().AddAsync(entity);
        await context.SaveChangesAsync();

        entity.Data = "User2";

        // Act
        await repository.UpdateAsync(entity);
        await context.SaveChangesAsync();

        // Assert
        var result = await context.Set<TestData>().FindAsync(entity.Id);
        Assert.NotNull(result);
        Assert.Equal("User2", result?.Data);
    }
}