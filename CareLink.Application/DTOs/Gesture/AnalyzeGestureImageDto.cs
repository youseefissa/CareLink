namespace CareLink.Application.DTOs.Gesture
{
    public class AnalyzeGestureResultDto
    {
        public string? Gesture { get; set; }
        public double Confidence { get; set; }
        public bool Detected { get; set; }
        public bool WasExecuted { get; set; }
    }
}