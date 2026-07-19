using CareLink.Application.Common;
using CareLink.Application.DTOs.Caregiver;

namespace CareLink.Application.Interfaces
{
    public interface ICaregiverDashboardService
    {
        Task<Result<IReadOnlyList<CaregiverDashboardDto>>> GetDashboardAsync(Guid caregiverProfileId);
        Task<Result> LinkPatientAsync(LinkCaregiverPatientDto request);
    }
}