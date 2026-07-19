using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
    {
        public void Configure(EntityTypeBuilder<NotificationLog> builder)
        {
            builder.ToTable("NotificationLogs");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title).IsRequired().HasMaxLength(150);
            builder.Property(n => n.Body).IsRequired().HasMaxLength(1000);
            builder.Property(n => n.Status).IsRequired();
        }
    }
}