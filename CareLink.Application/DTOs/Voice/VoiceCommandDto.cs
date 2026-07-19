namespace CareLink.Application.DTOs.Voice
{
    public class VoiceCommandDto
    {
        public Guid PatientProfileId { get; set; }
        public string RecognizedText { get; set; } = string.Empty;
    }
}