using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Serilog.Events;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class SOSEventConfiguration : IEntityTypeConfiguration<SOSEvent>
    {
        public void Configure(EntityTypeBuilder<SOSEvent> builder)
        {
            builder.ToTable("SOSEvents");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.TriggerSource).IsRequired().HasMaxLength(30);

            builder.HasIndex(s => new { s.PatientProfileId, s.Resolved });
        }
    }
}