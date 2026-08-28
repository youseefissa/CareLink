using CareLink.Application.Common;

namespace CareLink.Application.Interfaces
{
    public interface IContinuousMonitorService
    {
        Task<Result> ProcessFrameAsync(Guid patientProfileId, byte[] imageBytes, string fileName);
    }
}