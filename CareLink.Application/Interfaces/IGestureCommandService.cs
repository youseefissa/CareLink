using CareLink.Application.Common;
using CareLink.Application.DTOs.Gesture;

namespace CareLink.Application.Interfaces
{
    public interface IGestureCommandService
    {
        Task<Result> ProcessGestureAsync(GestureCommandDto request);
        Task<Result<AnalyzeGestureResultDto>> AnalyzeImageAsync(Guid patientProfileId, byte[] imageBytes, string fileName);
    }
}