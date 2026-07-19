using CareLink.Domain.Entities;

namespace CareLink.Domain.Interfaces.Repositories
{
    public interface ITrendReportRepository : IGenericRepository<TrendReport>
    {
        Task<IReadOnlyList<TrendReport>> GetByPatientIdAsync(Guid patientProfileId);
    }
}