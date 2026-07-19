namespace CareLink.Application.DTOs.SOS
{
    public class SOSEventDto
    {
        public Guid Id { get; set; }
        public Guid PatientProfileId { get; set; }
        public string TriggerSource { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool Resolved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}