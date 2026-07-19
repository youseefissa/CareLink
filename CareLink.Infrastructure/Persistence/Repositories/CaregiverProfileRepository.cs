using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class CaregiverProfileRepository : GenericRepository<CaregiverProfile>, ICaregiverProfileRepository
    {
        public CaregiverProfileRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<CaregiverProfile?> GetByUserIdAsync(Guid userId) =>
            await _dbSet.SingleOrDefaultAsync(c => c.UserId == userId);

        public async Task<CaregiverProfile?> GetWithLinksAsync(Guid caregiverProfileId) =>
            await _dbSet
                .Include(c => c.PatientLinks)
                .SingleOrDefaultAsync(c => c.Id == caregiverProfileId);
    }
}