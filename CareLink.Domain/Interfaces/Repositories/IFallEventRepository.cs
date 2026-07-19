using CareLink.Domain.Entities;

namespace CareLink.Domain.Interfaces.Repositories
{
    public interface IFallEventRepository : IGenericRepository<FallEvent>
    {
        Task<IReadOnlyList<FallEvent>> GetByPatientIdAsync(Guid patientProfileId);
        Task<int> CountFallsInPeriodAsync(Guid patientProfileId, DateTime start, DateTime end);
        Task<IReadOnlyList<FallEvent>> GetRecentFallsAsync(Guid patientProfileId, int count);
    }
}