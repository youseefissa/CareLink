using CareLink.Domain.Entities;

namespace CareLink.Domain.Interfaces.Repositories
{
    public interface ISafetyRecommendationRepository : IGenericRepository<SafetyRecommendation>
    {
        Task<IReadOnlyList<SafetyRecommendation>> GetByPatientIdAsync(Guid patientProfileId);
    }
}