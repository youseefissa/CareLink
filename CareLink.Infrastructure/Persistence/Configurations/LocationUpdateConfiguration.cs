using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class LocationUpdateConfiguration : IEntityTypeConfiguration<LocationUpdate>
    {
        public void Configure(EntityTypeBuilder<LocationUpdate> builder)
        {
            builder.ToTable("LocationUpdates");

            builder.HasKey(l => l.Id);

            builder.HasIndex(l => new { l.PatientProfileId, l.RecordedAt });
        }
    }
}