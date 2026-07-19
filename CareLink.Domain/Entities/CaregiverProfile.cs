using CareLink.Domain.Entities.Common;

namespace CareLink.Domain.Entities
{
    public class CaregiverProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string RelationshipType { get; set; } = string.Empty;

        public ICollection<CaregiverPatientLink> PatientLinks { get; set; } = new List<CaregiverPatientLink>();
    }
}