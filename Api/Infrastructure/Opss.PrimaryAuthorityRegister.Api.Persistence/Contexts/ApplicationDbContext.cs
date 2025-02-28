using Microsoft.EntityFrameworkCore;
using Opss.PrimaryAuthorityRegister.Api.Domain.Common;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Common.Providers;
using System.Reflection;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;

/// <summary>
/// Database context for the application.
/// </summary>
public class ApplicationDbContext : DbContext
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    /// <summary>
    /// Test Data: A collection of test data to provide use-examples
    /// </summary>
    public DbSet<TestData> TestData => Set<TestData>();

    /// <summary>
    ///     User Identities: Links a user's identity (from the identity providers) to 
    ///     an entity within the PAR system.
    /// </summary>
    public DbSet<UserIdentity> UserIdentities => Set<UserIdentity>();

    /// <summary>
    ///     Roles: Provides a user's identity with a role, these roles drive front-end
    ///     behaviour.
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>
    ///     Creates the model and applies configurations from the executing assembly.
    /// </summary>
    /// <param name="modelBuilder">
    ///     The builder being used to construct the model for this context. Databases (and other extensions) typically
    ///     define extension methods on this object that allow you to configure aspects of the model that are specific
    ///     to a given database.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    ///     Actually save the changes to the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A task that represents the asynchronous save operation. The task result contains the
    ///     number of state entries written to the database.
    /// </returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        SetAuditProperties();
        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private void SetAuditProperties()
    {
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added && entry.Entity is BaseAuditableEntity)
            {
                ((BaseAuditableEntity)entry.Entity).CreatedDate = _dateTimeProvider.UtcNow;
            }

            if(entry.State == EntityState.Modified && entry.Entity is BaseAuditableEntity)
            {
                ((BaseAuditableEntity)entry.Entity).UpdatedDate = _dateTimeProvider.UtcNow;
            }
        }
    }
}
