using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class GestureCommandLogConfiguration : IEntityTypeConfiguration<GestureCommandLog>
    {
        public void Configure(EntityTypeBuilder<GestureCommandLog> builder)
        {
            builder.ToTable("GestureCommandLogs");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Confidence).IsRequired();
        }
    }
}