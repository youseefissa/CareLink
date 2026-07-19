using CareLink.Domain.Entities;

namespace CareLink.Domain.Interfaces.Repositories
{
    public interface ICaregiverPatientLinkRepository : IGenericRepository<CaregiverPatientLink>
    {
        Task<bool> LinkExistsAsync(Guid caregiverProfileId, Guid patientProfileId);
    }
}