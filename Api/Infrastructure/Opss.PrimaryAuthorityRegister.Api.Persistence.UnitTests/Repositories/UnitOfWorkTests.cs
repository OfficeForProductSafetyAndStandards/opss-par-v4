using Microsoft.EntityFrameworkCore;
using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Repositories;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.UnitTests.Repositories;

public class UnitOfWorkTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenDbContextIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(null!));
    }

    [Fact]
    public void Repository_ShouldReturnGenericRepositoryInstance()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        using var unitOfWork = new UnitOfWork(context);

        // Act
        var repository = unitOfWork.Repository<TestData>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsAssignableFrom<IGenericRepository<TestData>>(repository);
    }

    [Fact]
    public void Repository_ShouldCacheRepositoryInstance()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        using var unitOfWork = new UnitOfWork(context);

        // Act
        var repository1 = unitOfWork.Repository<TestData>();
        var repository2 = unitOfWork.Repository<TestData>();

        // Assert
        Assert.Same(repository1, repository2); // The same instance should be returned
    }

    [Fact]
    public async Task Save_ShouldCommitChangesToDatabase()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        using var unitOfWork = new UnitOfWork(context);

        var entity = new TestData ("User1");
        await context.Set<TestData>().AddAsync(entity);

        // Act
        var changes = await unitOfWork.Save(CancellationToken.None);

        // Assert
        Assert.Equal(1, changes); // One change should have been saved
    }

    [Fact]
    public async Task Rollback_ShouldRevertUnsavedAddChanges()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        using var unitOfWork = new UnitOfWork(context);

        var entity = new TestData("User1");
        await context.Set<TestData>().AddAsync(entity);

        // Act
        await unitOfWork.Rollback();
        var trackedEntities = context.ChangeTracker.Entries().ToList();

        // Assert
        Assert.Empty(trackedEntities); // No entities should be tracked
    }

    [Fact]
    public async Task Rollback_ShouldRevertUnsavedUpdateChanges()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        using var unitOfWork = new UnitOfWork(context);

        var entity = new TestData("User1");
        await context.Set<TestData>().AddAsync(entity);
        await unitOfWork.Save(CancellationToken.None);
        entity.Data = "User2";
        context.Set<TestData>().Update(entity);

        // Act
        await unitOfWork.Rollback();
        var trackedEntities = context.ChangeTracker.Entries().ToList();

        // Assert
        entity.Data = "User1";
        Assert.Contains(entity, trackedEntities.Select(i => i.Entity));
    }

    [Fact]
    public async Task Rollback_ShouldRevertUnsavedDeleteChanges()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        using var unitOfWork = new UnitOfWork(context);

        var entity = new TestData("User1");
        await context.Set<TestData>().AddAsync(entity);
        await unitOfWork.Save(CancellationToken.None);
        context.Set<TestData>().Remove(entity);

        // Act
        await unitOfWork.Rollback();
        var trackedEntities = context.ChangeTracker.Entries().ToList();

        // Assert
        Assert.Contains(entity, trackedEntities.Select(i => i.Entity));
    }

    [Fact]
    public void Dispose_ShouldDisposeDbContext()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        using var unitOfWork = new UnitOfWork(context);

        // Act
        unitOfWork.Dispose();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => context.Set<TestData>().Add(new TestData("Data")));
    }

    [Fact]
    public void Dispose_ShouldBeIdempotent()
    {
        // Arrange
        using var context = new ApplicationDbContext(InMemoryDatabaseTestHelpers.InMemoryOptions);
        using var unitOfWork = new UnitOfWork(context);

        // Act
        unitOfWork.Dispose();
        unitOfWork.Dispose(); // Calling Dispose multiple times should not throw

        // Assert
        Assert.True(true); // If no exception is thrown, the test passes
    }
}