using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class SafetyRecommendationRepository : GenericRepository<SafetyRecommendation>, ISafetyRecommendationRepository
    {
        public SafetyRecommendationRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<SafetyRecommendation>> GetByPatientIdAsync(Guid patientProfileId) =>
            await _dbSet
                .Where(r => r.PatientProfileId == patientProfileId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
    }
}