using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class FallEventRepository : GenericRepository<FallEvent>, IFallEventRepository
    {
        public FallEventRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<FallEvent>> GetByPatientIdAsync(Guid patientProfileId) =>
            await _dbSet
                .Where(f => f.PatientProfileId == patientProfileId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

        public async Task<int> CountFallsInPeriodAsync(Guid patientProfileId, DateTime start, DateTime end) =>
            await _dbSet.CountAsync(f =>
                f.PatientProfileId == patientProfileId &&
                f.IsFall &&
                f.CreatedAt >= start &&
                f.CreatedAt <= end);

        public async Task<IReadOnlyList<FallEvent>> GetRecentFallsAsync(Guid patientProfileId, int count) =>
            await _dbSet
                .Where(f => f.PatientProfileId == patientProfileId && f.IsFall)
                .OrderByDescending(f => f.CreatedAt)
                .Take(count)
                .ToListAsync();
    }
}