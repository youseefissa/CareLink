namespace CareLink.Application.DTOs.Alert
{
    public class AlertBroadcastDto
    {
        public Guid Id { get; set; }
        public Guid PatientProfileId { get; set; }
        public int Type { get; set; }
        public int Severity { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}