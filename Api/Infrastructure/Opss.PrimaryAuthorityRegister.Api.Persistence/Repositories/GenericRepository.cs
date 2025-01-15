using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Common;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseAuditableEntity
{
    private readonly ApplicationDbContext dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<T> AddAsync(T entity)
    {
        await dbContext.Set<T>().AddAsync(entity).ConfigureAwait(false);
        return entity;
    }

    public Task DeleteAsync(T entity)
    {
        dbContext.Set<T>().Remove(entity);
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
        return await dbContext.Set<T>().FindAsync(Id).ConfigureAwait(false);
    }

    public Task UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        T? exist = GetByIdAsync(entity.Id).GetAwaiter().GetResult();

        if (exist != null)
        {
            dbContext.Entry(exist).CurrentValues.SetValues(entity);
        }

        return Task.CompletedTask;
    }
}
