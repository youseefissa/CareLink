namespace CareLink.Application.DTOs.Caregiver
{
    public class CaregiverDashboardDto
    {
        public Guid PatientProfileId { get; set; }
        public string PatientFullName { get; set; } = string.Empty;

        public double? LastLatitude { get; set; }
        public double? LastLongitude { get; set; }
        public DateTime? LastLocationAt { get; set; }

        public DateTime? LastActivityAt { get; set; }
        public DateTime? LastFallAt { get; set; }

        public int UnresolvedAlertsCount { get; set; }
        public int FallsThisWeek { get; set; }
    }
}