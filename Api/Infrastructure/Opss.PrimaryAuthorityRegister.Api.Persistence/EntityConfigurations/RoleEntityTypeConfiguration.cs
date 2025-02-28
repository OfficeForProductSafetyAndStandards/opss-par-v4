using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.EntityConfigurations;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnType("uuid");
        builder.Property(u => u.CreatedDate).HasColumnType("timestamptz").IsRequired(false);
        builder.Property(u => u.UpdatedDate).HasColumnType("timestamptz").IsRequired(false);
        builder.Property(u => u.Name).HasColumnType("varchar(100)").IsRequired();

        builder.HasIndex(u => u.Name).IsUnique();

        builder.HasMany(u => u.UserIdentities).WithMany(r => r.Roles);
    }
}
