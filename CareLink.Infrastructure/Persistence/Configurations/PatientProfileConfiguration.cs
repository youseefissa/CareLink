using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class PatientProfileConfiguration : IEntityTypeConfiguration<PatientProfile>
    {
        public void Configure(EntityTypeBuilder<PatientProfile> builder)
        {
            builder.ToTable("PatientProfiles");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.MedicalNotes).HasMaxLength(2000);
            builder.Property(p => p.EmergencyContactPhone).HasMaxLength(20);

            builder.HasMany(p => p.FallEvents)
                .WithOne(f => f.PatientProfile)
                .HasForeignKey(f => f.PatientProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.SOSEvents)
                .WithOne(s => s.PatientProfile)
                .HasForeignKey(s => s.PatientProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.ActivityLogs)
                .WithOne(a => a.PatientProfile)
                .HasForeignKey(a => a.PatientProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.LocationUpdates)
                .WithOne(l => l.PatientProfile)
                .HasForeignKey(l => l.PatientProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}