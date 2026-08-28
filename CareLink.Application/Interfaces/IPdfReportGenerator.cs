namespace CareLink.Application.Interfaces
{
    public interface IPdfReportGenerator
    {
        byte[] GenerateTrendReportPdf(TrendReportPdfData data);
    }

    public class TrendReportPdfData
    {
        public string PatientFullName { get; set; } = string.Empty;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int TotalFalls { get; set; }
        public double AverageDailyActivity { get; set; }
        public int MedicationConfirmationsCount { get; set; }
        public int MedicationMissedCount { get; set; }
        public int InactivityEventsCount { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}