using Microsoft.EntityFrameworkCore;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Common;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;
using System.Collections;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext dbContext;
    private Hashtable? repositories;
    private bool disposed;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    public IGenericRepository<T> Repository<T>() where T : BaseAuditableEntity
    {
        repositories ??= new Hashtable();

        var type = typeof(T).Name;

        if (!repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<T>);
            
            var repositoryInstance = Activator.CreateInstance(repositoryType, dbContext);

            repositories.Add(type, repositoryInstance);
        }

        var repository = (IGenericRepository<T>?)repositories[type];

        if (repository == null)
        {
            throw new NotImplementedException($"Repository for {type} not found");
        }

        return repository;
    }

    public Task Rollback()
    {
        foreach (var entry in dbContext.ChangeTracker.Entries().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.State = EntityState.Unchanged; // Revert modifications
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;  // Detach newly added entities
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged; // Revert deletions
                    break;
            }
        }
        return Task.CompletedTask;
    }

    public async Task<int> Save(CancellationToken cancellationToken)
    {
        return await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            dbContext.Dispose();
        }

        disposed = true;
    }
}
