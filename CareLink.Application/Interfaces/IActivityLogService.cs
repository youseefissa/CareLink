using CareLink.Application.Common;
using CareLink.Application.DTOs.ActivityLog;

namespace CareLink.Application.Interfaces
{
    public interface IActivityLogService
    {
        Task<Result<ActivityLogDto>> LogActivityAsync(CreateActivityLogDto request);
        Task<Result<IReadOnlyList<ActivityLogDto>>> GetHistoryAsync(Guid patientProfileId, DateTime? from = null, DateTime? to = null);
    }
}