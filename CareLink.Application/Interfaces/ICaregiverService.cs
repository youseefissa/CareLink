using CareLink.Application.Common;
using CareLink.Application.DTOs.Caregiver;

namespace CareLink.Application.Interfaces
{
    public interface ICaregiverService
    {
        Task<Result<CaregiverProfileDto>> CreateProfileAsync(CreateCaregiverProfileDto request);
        Task<Result<CaregiverProfileDto>> GetByIdAsync(Guid caregiverProfileId);
        Task<Result<CaregiverProfileDto>> GetMyProfileAsync();
    }
}