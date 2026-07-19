using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLink.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.PhoneNumber).HasMaxLength(20);
            builder.Property(u => u.Role).IsRequired();

            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasOne(u => u.PatientProfile)
                .WithOne(p => p.User)
                .HasForeignKey<PatientProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.CaregiverProfile)
                .WithOne(c => c.User)
                .HasForeignKey<CaregiverProfile>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}