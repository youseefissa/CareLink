using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class AlertConfiguration : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.ToTable("Alerts");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Message).IsRequired().HasMaxLength(500);
            builder.Property(a => a.Type).IsRequired();
            builder.Property(a => a.Severity).IsRequired();

            builder.HasIndex(a => new { a.PatientProfileId, a.IsResolved });
        }
    }
}