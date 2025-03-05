using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.EntityConfigurations;

public class RegulatoryFunctionTypeConfiguration : IEntityTypeConfiguration<RegulatoryFunction>
{
    public void Configure(EntityTypeBuilder<RegulatoryFunction> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).HasColumnType("uuid");
        builder.Property(a => a.CreatedDate).HasColumnType("timestamptz").IsRequired(false);
        builder.Property(a => a.UpdatedDate).HasColumnType("timestamptz").IsRequired(false);

        builder.Property(a => a.Name).HasColumnType("varchar(1024)").IsRequired();

        builder.HasMany(a => a.Authorities)
               .WithMany(f => f.RegulatoryFunctions);
    }
}