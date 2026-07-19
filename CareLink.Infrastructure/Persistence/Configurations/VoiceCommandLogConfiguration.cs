using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class VoiceCommandLogConfiguration : IEntityTypeConfiguration<VoiceCommandLog>
    {
        public void Configure(EntityTypeBuilder<VoiceCommandLog> builder)
        {
            builder.ToTable("VoiceCommandLogs");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.RecognizedText).IsRequired().HasMaxLength(500);
            builder.Property(v => v.MatchedCommand).HasMaxLength(100);
        }
    }
}