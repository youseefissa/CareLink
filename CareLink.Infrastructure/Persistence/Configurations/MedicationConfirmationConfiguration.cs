using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class MedicationConfirmationConfiguration : IEntityTypeConfiguration<MedicationConfirmation>
    {
        public void Configure(EntityTypeBuilder<MedicationConfirmation> builder)
        {
            builder.ToTable("MedicationConfirmations");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.MedicationName).IsRequired().HasMaxLength(150);
        }
    }
}