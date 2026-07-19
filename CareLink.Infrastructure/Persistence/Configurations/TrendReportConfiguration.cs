using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class TrendReportConfiguration : IEntityTypeConfiguration<TrendReport>
    {
        public void Configure(EntityTypeBuilder<TrendReport> builder)
        {
            builder.ToTable("TrendReports");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.GeneratedPdfPath).HasMaxLength(300);
        }
    }
}