using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class CaregiverProfileConfiguration : IEntityTypeConfiguration<CaregiverProfile>
    {
        public void Configure(EntityTypeBuilder<CaregiverProfile> builder)
        {
            builder.ToTable("CaregiverProfiles");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.RelationshipType).HasMaxLength(50);
        }
    }
}