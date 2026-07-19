namespace CareLink.Dashboard.Models
{
    public class CaregiverDashboardItem
    {
        public Guid PatientProfileId { get; set; }
        public string PatientFullName { get; set; } = string.Empty;
        public DateTime? LastActivityAt { get; set; }
        public DateTime? LastFallAt { get; set; }
        public int UnresolvedAlertsCount { get; set; }
        public int FallsThisWeek { get; set; }
    }

    public class AlertItem
    {
        public Guid Id { get; set; }
        public int Type { get; set; }
        public int Severity { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TrendReportItem
    {
        public Guid Id { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int TotalFalls { get; set; }
        public double AverageDailyActivity { get; set; }
        public int InactivityEventsCount { get; set; }
    }

    public class ApiErrorResponse
    {
        public List<string> Errors { get; set; } = new();
    }
    public class FallEventItem
    {
        public Guid Id { get; set; }
        public bool IsFall { get; set; }
        public double Confidence { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SOSEventItem
    {
        public Guid Id { get; set; }
        public string TriggerSource { get; set; } = string.Empty;
        public bool Resolved { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ActivityLogItem
    {
        public Guid Id { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime OccurredAt { get; set; }
    }

    public class SafetyRecommendationItem
    {
        public Guid Id { get; set; }
        public string RecommendationText { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsAcknowledged { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PatientProfileDetail
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? MedicalNotes { get; set; }
        public string? EmergencyContactPhone { get; set; }
    }
}