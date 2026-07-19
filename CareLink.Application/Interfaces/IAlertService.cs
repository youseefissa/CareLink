using CareLink.Application.Common;
using CareLink.Application.DTOs.Alert;

namespace CareLink.Application.Interfaces
{
    public interface IAlertService
    {
        Task<Result<IReadOnlyList<AlertDto>>> GetForPatientAsync(Guid patientProfileId);
        Task<Result> ResolveAsync(Guid alertId);
    }
}