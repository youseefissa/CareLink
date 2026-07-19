using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class CaregiverPatientLinkConfiguration : IEntityTypeConfiguration<CaregiverPatientLink>
    {
        public void Configure(EntityTypeBuilder<CaregiverPatientLink> builder)
        {
            builder.ToTable("CaregiverPatientLinks");

            builder.HasKey(l => l.Id);

            builder.HasOne(l => l.CaregiverProfile)
                .WithMany(c => c.PatientLinks)
                .HasForeignKey(l => l.CaregiverProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.PatientProfile)
                .WithMany(p => p.CaregiverLinks)
                .HasForeignKey(l => l.PatientProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(l => new { l.CaregiverProfileId, l.PatientProfileId }).IsUnique();
        }
    }
}