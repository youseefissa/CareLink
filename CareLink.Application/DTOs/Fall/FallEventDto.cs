namespace CareLink.Application.DTOs.Fall
{
    public class FallEventDto
    {
        public Guid Id { get; set; }
        public Guid PatientProfileId { get; set; }
        public bool IsFall { get; set; }
        public double Confidence { get; set; }
        public string? FallType { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}