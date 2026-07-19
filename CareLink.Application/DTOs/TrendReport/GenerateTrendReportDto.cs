namespace CareLink.Application.DTOs.TrendReport
{
    public class GenerateTrendReportDto
    {
        public Guid PatientProfileId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}