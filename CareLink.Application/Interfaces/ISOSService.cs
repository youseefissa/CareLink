using CareLink.Application.Common;
using CareLink.Application.DTOs.SOS;

namespace CareLink.Application.Interfaces
{
    public interface ISOSService
    {
        Task<Result<SOSEventDto>> TriggerAsync(CreateSOSEventDto request);
        Task<Result> ResolveAsync(Guid sosEventId);
        Task<Result<IReadOnlyList<SOSEventDto>>> GetHistoryAsync(Guid patientProfileId);
    }
}