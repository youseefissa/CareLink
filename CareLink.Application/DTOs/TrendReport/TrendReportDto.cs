namespace CareLink.Application.DTOs.TrendReport
{
    public class TrendReportDto
    {
        public Guid Id { get; set; }
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