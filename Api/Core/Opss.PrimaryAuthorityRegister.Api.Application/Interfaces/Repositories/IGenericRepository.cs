using Microsoft.EntityFrameworkCore.ChangeTracking;
using Opss.PrimaryAuthorityRegister.Api.Domain.Common.Interfaces;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;

/// <summary>
/// Interface definining all the generic repository functions that 
/// will be provided by all repositories.
/// </summary>
/// <typeparam name="T">The domain model type the repository handles.</typeparam>
public interface IGenericRepository<T> where T : class, IEntity
{
    /// <summary>
    /// Provide access to a queryable list of entities.
    /// </summary>
    IQueryable<T> Entities { get; }

    /// <summary>
    /// Provides access to non-commited data
    /// </summary>
    /// <returns>Local changes waiting to be committed</returns>
    ReadOnlyCollection<T> Local();

    /// <summary>
    /// Return an item by it's Id.
    /// </summary>
    /// <param name="Id">The item to find.</param>
    /// <returns>If found, the item, otherwise nul.</returns>
    Task<T?> GetByIdAsync(Guid Id, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Add an item to the repository.
    /// </summary>
    /// <param name="entity">The item to add to the repository.</param>
    /// <returns>The added item.</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Update the provided item in the repository.
    /// </summary>
    /// <param name="entity">The item to update.</param>
    /// <returns>A completed task when the item has been updated.</returns>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Remove an item from the repository.
    /// </summary>
    /// <param name="entity">The item to remove.</param>
    /// <returns>A completed talk when the item has been removed.</returns>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Remove an item from the repository by it's Id.
    /// </summary>
    /// <param name="Id">The Id of the item to remove.</param>
    /// <returns>A completed talk when the item has been removed.</returns>
    Task DeleteByIdAsync(Guid Id);
}
