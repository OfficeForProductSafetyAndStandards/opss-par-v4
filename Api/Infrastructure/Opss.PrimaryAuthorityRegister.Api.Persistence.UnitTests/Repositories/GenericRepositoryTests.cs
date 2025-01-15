using Microsoft.EntityFrameworkCore;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Repositories;

public class GenericRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> InMemoryOptions => new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    [Fact]
    public async Task AddAsync_ShouldAddEntityToDbContext()
    {
        // Arrange
        var options = InMemoryOptions;
        using var context = new ApplicationDbContext(options);
        var repository = new GenericRepository<TestData>(context);

        var entity = new TestData("Test");

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
        var options = InMemoryOptions;
        using var context = new ApplicationDbContext(options);
        var repository = new GenericRepository<TestData>(context);

        var entity = new TestData("User1");
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
        var options = InMemoryOptions;
        using var context = new ApplicationDbContext(options);
        var repository = new GenericRepository<TestData>(context);

        var entity = new TestData("User1");
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
        var options = InMemoryOptions;
        using var context = new ApplicationDbContext(options);
        var repository = new GenericRepository<TestData>(context);

        var entity = new TestData("User1");
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
        var options = InMemoryOptions;
        using var context = new ApplicationDbContext(options);
        var repository = new GenericRepository<TestData>(context);

        var entity = new TestData("User1");
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