using CareLink.Domain.Entities;

namespace CareLink.Domain.Interfaces.Repositories
{
    public interface IPatientProfileRepository : IGenericRepository<PatientProfile>
    {
        Task<PatientProfile?> GetByUserIdAsync(Guid userId);
        Task<PatientProfile?> GetWithDetailsAsync(Guid patientProfileId);
        Task<IReadOnlyList<PatientProfile>> GetByCaregiverIdAsync(Guid caregiverProfileId);
    }
}