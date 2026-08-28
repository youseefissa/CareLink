namespace CareLink.Application.Interfaces
{
    public interface IAiServiceClient
    {
        Task<AiGestureResult> AnalyzeHandGestureAsync(byte[] imageBytes, string fileName);
        Task<AiFallResult> AnalyzeFallAsync(byte[] imageBytes, string fileName);
        Task<AiContinuousMonitorResult> AnalyzeContinuousFrameAsync(Guid patientProfileId, byte[] imageBytes, string fileName);
    }

    public class AiGestureResult
    {
        public string? Gesture { get; set; }
        public double Confidence { get; set; }
        public bool Detected { get; set; }
    }

    public class AiFallResult
    {
        public bool IsFall { get; set; }
        public double Confidence { get; set; }
        public bool Detected { get; set; }
    }

    public class AiContinuousMonitorResult
    {
        public bool Emergency { get; set; }
        public string? Reason { get; set; }
        public bool Detected { get; set; }
        public bool IsFall { get; set; }
        public double SecondsSinceMovement { get; set; }
    }
}