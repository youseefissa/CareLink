using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.ToTable("ActivityLogs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.ActivityType).IsRequired().HasMaxLength(50);
            builder.Property(a => a.Details).HasMaxLength(500);

            builder.HasIndex(a => new { a.PatientProfileId, a.OccurredAt });
        }
    }
}