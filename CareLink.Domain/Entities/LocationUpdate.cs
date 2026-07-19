using CareLink.Domain.Entities.Common;

namespace CareLink.Domain.Entities
{
    public class LocationUpdate : BaseEntity
    {
        public Guid PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; } = null!;

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}