using CareLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using System.Reflection;

namespace CareLink.Infrastructure.Persistence
{
    public class CareLinkDbContext : DbContext
    {
        public CareLinkDbContext(DbContextOptions<CareLinkDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<PatientProfile> PatientProfiles => Set<PatientProfile>();
        public DbSet<CaregiverProfile> CaregiverProfiles => Set<CaregiverProfile>();
        public DbSet<CaregiverPatientLink> CaregiverPatientLinks => Set<CaregiverPatientLink>();
        public DbSet<FallEvent> FallEvents => Set<FallEvent>();
        public DbSet<SOSEvent> SOSEvents => Set<SOSEvent>();
        public DbSet<VoiceCommandLog> VoiceCommandLogs => Set<VoiceCommandLog>();
        public DbSet<GestureCommandLog> GestureCommandLogs => Set<GestureCommandLog>();
        public DbSet<MedicationConfirmation> MedicationConfirmations => Set<MedicationConfirmation>();
        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
        public DbSet<SafetyRecommendation> SafetyRecommendations => Set<SafetyRecommendation>();
        public DbSet<Alert> Alerts => Set<Alert>();
        public DbSet<TrendReport> TrendReports => Set<TrendReport>();
        public DbSet<LocationUpdate> LocationUpdates => Set<LocationUpdate>();
        public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Global query filter: hide soft-deleted rows automatically
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<PatientProfile>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<CaregiverProfile>().HasQueryFilter(e => !e.IsDeleted);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            ApplyTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyTimestamps()
        {
            var entries = ChangeTracker.Entries<Domain.Entities.Common.BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}