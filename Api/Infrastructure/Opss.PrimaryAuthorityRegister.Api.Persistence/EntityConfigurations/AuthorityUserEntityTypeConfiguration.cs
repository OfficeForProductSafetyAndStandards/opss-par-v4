using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.EntityConfigurations;

public class AuthorityUserEntityTypeConfiguration : IEntityTypeConfiguration<AuthorityUser>
{
    public void Configure(EntityTypeBuilder<AuthorityUser> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).HasColumnType("uuid");
        builder.Property(a => a.CreatedDate).HasColumnType("timestamptz").IsRequired(false);
        builder.Property(a => a.UpdatedDate).HasColumnType("timestamptz").IsRequired(false);

        builder.HasOne(a => a.UserIdentity)
               .WithOne(f => f.AuthorityUser)
               .HasForeignKey<UserIdentity>()
               .IsRequired(false);

        builder.HasOne(u => u.Authority)
               .WithMany(o => o.AuthorityUsers)
               .HasForeignKey(u => u.AuthorityId)
               .IsRequired(false);
    }
}
