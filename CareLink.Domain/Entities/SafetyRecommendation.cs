using CareLink.Domain.Entities.Common;

namespace CareLink.Domain.Entities
{
    public class SafetyRecommendation : BaseEntity
    {
        public Guid PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; } = null!;

        public string RecommendationText { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsAcknowledged { get; set; }
    }
}