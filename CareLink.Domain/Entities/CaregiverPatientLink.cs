using CareLink.Domain.Entities.Common;

namespace CareLink.Domain.Entities
{
    public class CaregiverPatientLink : BaseEntity
    {
        public Guid CaregiverProfileId { get; set; }
        public CaregiverProfile CaregiverProfile { get; set; } = null!;

        public Guid PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; } = null!;

        public bool IsPrimaryCaregiver { get; set; }
    }
}