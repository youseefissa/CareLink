namespace CareLink.Application.DTOs.Fall
{
    public class CreateFallEventDto
    {
        public Guid PatientProfileId { get; set; }
        public bool IsFall { get; set; }
        public double Confidence { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}