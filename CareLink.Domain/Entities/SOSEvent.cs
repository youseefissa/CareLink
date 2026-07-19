using CareLink.Domain.Entities.Common;

namespace CareLink.Domain.Entities
{
    public class SOSEvent : BaseEntity
    {
        public Guid PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; } = null!;

        public string TriggerSource { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool Resolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}