using Microsoft.EntityFrameworkCore.ChangeTracking;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Common;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;
using System.Collections.ObjectModel;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseAuditableEntity
{
    private readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<T> Entities => _dbContext.Set<T>();

    public ReadOnlyCollection<T> Local()
    {
        return new ReadOnlyCollection<T>(_dbContext.Set<T>().Local.ToList());
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity).ConfigureAwait(false);
        return entity;
    }

    public Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteByIdAsync(Guid Id)
    {
        T? entity = GetByIdAsync(Id).GetAwaiter().GetResult();

        if (entity != null)
        {
            DeleteAsync(entity);
        }

        return Task.CompletedTask;
    }

    public async Task<T?> GetByIdAsync(Guid Id)
    {
        return await _dbContext.Set<T>().FindAsync(Id).ConfigureAwait(false);
    }

    public Task UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        T? exist = GetByIdAsync(entity.Id).GetAwaiter().GetResult();

        if (exist != null)
        {
            _dbContext.Entry(exist).CurrentValues.SetValues(entity);
        }

        return Task.CompletedTask;
    }
}
