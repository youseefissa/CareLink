namespace CareLink.Application.DTOs.SOS
{
    public class CreateSOSEventDto
    {
        public Guid PatientProfileId { get; set; }
        public string TriggerSource { get; set; } = string.Empty; // Button, Voice, Gesture
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}