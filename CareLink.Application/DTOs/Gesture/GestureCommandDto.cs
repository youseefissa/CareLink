using CareLink.Domain.Enums;

namespace CareLink.Application.DTOs.Gesture
{
    public class GestureCommandDto
    {
        public Guid PatientProfileId { get; set; }
        public GestureType Gesture { get; set; }
        public double Confidence { get; set; }
    }
}