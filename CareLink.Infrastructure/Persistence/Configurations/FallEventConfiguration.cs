using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class FallEventConfiguration : IEntityTypeConfiguration<FallEvent>
    {
        public void Configure(EntityTypeBuilder<FallEvent> builder)
        {
            builder.ToTable("FallEvents");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Confidence).IsRequired();
            builder.Property(f => f.FallType).HasMaxLength(50);

            builder.HasIndex(f => new { f.PatientProfileId, f.CreatedAt });
        }
    }
}