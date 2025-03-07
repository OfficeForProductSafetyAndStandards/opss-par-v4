using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.EntityConfigurations
{
    public class UserProfileEntityTypeConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            
            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.Id).HasColumnType("uuid");
            builder.Property(u => u.CreatedDate).HasColumnType("timestamptz").IsRequired(false);
            builder.Property(u => u.UpdatedDate).HasColumnType("timestamptz").IsRequired(false);
            
            builder.Property(u => u.HasAcceptedTermsAndConditions).HasColumnType("boolean").IsRequired();
            
            builder.HasOne(u => u.UserIdentity)
                .WithOne(f => f.UserProfile)
                .HasForeignKey<UserIdentity>()
                .IsRequired(false);
        }
    }
}