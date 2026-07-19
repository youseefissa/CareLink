using CareLink.Domain.Entities;

namespace CareLink.Domain.Interfaces.Repositories
{
    public interface IActivityLogRepository : IGenericRepository<ActivityLog>
    {
        Task<IReadOnlyList<ActivityLog>> GetByPatientIdAsync(Guid patientProfileId, DateTime? from = null, DateTime? to = null);
        Task<DateTime?> GetLastActivityTimeAsync(Guid patientProfileId);
        Task<double> GetAverageDailyActivityAsync(Guid patientProfileId, DateTime start, DateTime end);
    }
}