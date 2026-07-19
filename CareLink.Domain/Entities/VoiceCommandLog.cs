using CareLink.Domain.Entities.Common;

namespace CareLink.Domain.Entities
{
    public class VoiceCommandLog : BaseEntity
    {
        public Guid PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; } = null!;

        public string RecognizedText { get; set; } = string.Empty;
        public string MatchedCommand { get; set; } = string.Empty;
        public bool WasExecuted { get; set; }
    }
}