using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class PatientProfileRepository : GenericRepository<PatientProfile>, IPatientProfileRepository
    {
        public PatientProfileRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<PatientProfile?> GetByUserIdAsync(Guid userId) =>
            await _dbSet.SingleOrDefaultAsync(p => p.UserId == userId);

        public async Task<PatientProfile?> GetWithDetailsAsync(Guid patientProfileId) =>
            await _dbSet
                .Include(p => p.User)
                .Include(p => p.CaregiverLinks)
                .SingleOrDefaultAsync(p => p.Id == patientProfileId);

        public async Task<IReadOnlyList<PatientProfile>> GetByCaregiverIdAsync(Guid caregiverProfileId) =>
            await _dbSet
                .Include(p => p.User)
                .Where(p => p.CaregiverLinks.Any(l => l.CaregiverProfileId == caregiverProfileId))
                .ToListAsync();
    }
}