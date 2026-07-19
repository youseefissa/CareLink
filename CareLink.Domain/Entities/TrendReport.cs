using CareLink.Domain.Entities.Common;

namespace CareLink.Domain.Entities
{
    public class TrendReport : BaseEntity
    {
        public Guid PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; } = null!;

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public int TotalFalls { get; set; }
        public double AverageDailyActivity { get; set; }
        public int MedicationConfirmationsCount { get; set; }
        public int MedicationMissedCount { get; set; }
        public int InactivityEventsCount { get; set; }

        public string? GeneratedPdfPath { get; set; }
    }
}