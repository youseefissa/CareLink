using CareLink.Domain.Entities.Common;

namespace CareLink.Domain.Entities
{
    public class PatientProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }
        public string? MedicalNotes { get; set; }
        public bool HasVisualImpairment { get; set; }
        public bool HasHearingImpairment { get; set; }
        public string? EmergencyContactPhone { get; set; }

        public TimeSpan SleepWindowStart { get; set; } = new TimeSpan(22, 0, 0);
        public TimeSpan SleepWindowEnd { get; set; } = new TimeSpan(7, 0, 0);

        public ICollection<CaregiverPatientLink> CaregiverLinks { get; set; } = new List<CaregiverPatientLink>();
        public ICollection<FallEvent> FallEvents { get; set; } = new List<FallEvent>();
        public ICollection<SOSEvent> SOSEvents { get; set; } = new List<SOSEvent>();
        public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
        public ICollection<LocationUpdate> LocationUpdates { get; set; } = new List<LocationUpdate>();
    }
}