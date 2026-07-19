using CareLink.Domain.Entities;

namespace CareLink.Domain.Interfaces.Repositories
{
    public interface ISOSEventRepository : IGenericRepository<SOSEvent>
    {
        Task<IReadOnlyList<SOSEvent>> GetByPatientIdAsync(Guid patientProfileId);
        Task<IReadOnlyList<SOSEvent>> GetUnresolvedAsync(Guid patientProfileId);
    }
}