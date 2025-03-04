using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.EntityConfigurations;

public class AuthorityEntityTypeConfiguration : IEntityTypeConfiguration<Authority>
{
    public void Configure(EntityTypeBuilder<Authority> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).HasColumnType("uuid");
        builder.Property(a => a.CreatedDate).HasColumnType("timestamptz").IsRequired(false);
        builder.Property(a => a.UpdatedDate).HasColumnType("timestamptz").IsRequired(false);

        builder.Property(a => a.Name).HasColumnType("varchar(1024)").IsRequired();

        builder.HasMany(a => a.AuthorityUsers)
               .WithOne(f => f.Authority)
               .HasForeignKey(f => f.AuthorityId)
               .IsRequired();

        builder.HasMany(a => a.RegulatoryFunctions)
               .WithMany(f => f.Authorities);
    }
}
