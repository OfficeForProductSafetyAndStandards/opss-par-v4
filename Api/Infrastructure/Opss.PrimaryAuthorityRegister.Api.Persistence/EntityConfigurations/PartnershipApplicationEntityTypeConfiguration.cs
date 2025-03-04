using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.EntityConfigurations;

public class PartnershipApplicationEntityTypeConfiguration : IEntityTypeConfiguration<PartnershipApplication>
{
    public void Configure(EntityTypeBuilder<PartnershipApplication> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).HasColumnType("uuid");
        builder.Property(a => a.CreatedDate).HasColumnType("timestamptz").IsRequired(false);
        builder.Property(a => a.UpdatedDate).HasColumnType("timestamptz").IsRequired(false);

        builder.HasOne(u => u.Authority)
               .WithMany(o => o.PartnershipApplications)
               .HasForeignKey(u => u.AuthorityId)
               .IsRequired(false);
    }
}
