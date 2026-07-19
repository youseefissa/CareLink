namespace CareLink.Application.DTOs.Patient
{
    public class PatientProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? MedicalNotes { get; set; }
        public bool HasVisualImpairment { get; set; }
        public bool HasHearingImpairment { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public TimeSpan SleepWindowStart { get; set; }
        public TimeSpan SleepWindowEnd { get; set; }
    }
}