using CareLink.Application.Common;
using CareLink.Application.DTOs.Fall;

namespace CareLink.Application.Interfaces
{
    public interface IFallDetectionService
    {
        Task<Result<FallEventDto>> RecordFallEventAsync(CreateFallEventDto request);
        Task<Result<IReadOnlyList<FallEventDto>>> GetHistoryAsync(Guid patientProfileId);
    }
}