using CareLink.Domain.Entities;

namespace CareLink.Domain.Interfaces.Repositories
{
    public interface IAlertRepository : IGenericRepository<Alert>
    {
        Task<IReadOnlyList<Alert>> GetByPatientIdAsync(Guid patientProfileId);
        Task<IReadOnlyList<Alert>> GetUnresolvedAsync(Guid patientProfileId);
    }
}