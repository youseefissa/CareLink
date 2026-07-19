using CareLink.Domain.Entities.Common;
using CareLink.Domain.Enums;

namespace CareLink.Domain.Entities
{
    public class GestureCommandLog : BaseEntity
    {
        public Guid PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; } = null!;

        public GestureType Gesture { get; set; }
        public double Confidence { get; set; }
        public bool WasExecuted { get; set; }
    }
}