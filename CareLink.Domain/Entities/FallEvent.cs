using CareLink.Domain.Entities.Common;

namespace CareLink.Domain.Entities
{
    public class FallEvent : BaseEntity
    {
        public Guid PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; } = null!;

        public bool IsFall { get; set; }
        public double Confidence { get; set; }
        public string? FallType { get; set; } = null;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public bool CaregiverNotified { get; set; }
    }
}