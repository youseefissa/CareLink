namespace CareLink.Application.Interfaces
{
    // Contract for talking to the Python FastAPI AI service (fall detection inference)
    public interface IAiServiceClient
    {
        Task<AiFallPredictionResult> PredictFallAsync(Stream sensorDataOrFrame);
    }

    public class AiFallPredictionResult
    {
        public bool IsFall { get; set; }
        public double Confidence { get; set; }
    }
}