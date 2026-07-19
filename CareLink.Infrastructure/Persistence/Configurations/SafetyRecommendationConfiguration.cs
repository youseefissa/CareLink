using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class SafetyRecommendationConfiguration : IEntityTypeConfiguration<SafetyRecommendation>
    {
        public void Configure(EntityTypeBuilder<SafetyRecommendation> builder)
        {
            builder.ToTable("SafetyRecommendations");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.RecommendationText).IsRequired().HasMaxLength(500);
            builder.Property(s => s.Category).HasMaxLength(50);
        }
    }
}