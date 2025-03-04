using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.EntityConfigurations;

public class UserIdentityEntityTypeConfiguration : IEntityTypeConfiguration<UserIdentity>
{
    public void Configure(EntityTypeBuilder<UserIdentity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnType("uuid");
        builder.Property(u => u.CreatedDate).HasColumnType("timestamptz").IsRequired(false);
        builder.Property(u => u.UpdatedDate).HasColumnType("timestamptz").IsRequired(false);

        builder.Property(u => u.EmailAddress).HasColumnType("varchar(320)").IsRequired();

        builder.HasIndex(u => u.EmailAddress).IsUnique();

        builder.HasMany(u => u.Roles).WithMany(r => r.UserIdentities);
        builder.HasOne(u => u.AuthorityUser)
               .WithOne(c => c.UserIdentity)
               .HasForeignKey<AuthorityUser>()
               .IsRequired(false);
    }
}
