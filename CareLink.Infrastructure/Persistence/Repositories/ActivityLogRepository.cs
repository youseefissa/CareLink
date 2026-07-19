using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class ActivityLogRepository : GenericRepository<ActivityLog>, IActivityLogRepository
    {
        public ActivityLogRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<ActivityLog>> GetByPatientIdAsync(Guid patientProfileId, DateTime? from = null, DateTime? to = null)
        {
            var query = _dbSet.Where(a => a.PatientProfileId == patientProfileId);

            if (from.HasValue)
                query = query.Where(a => a.OccurredAt >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.OccurredAt <= to.Value);

            return await query.OrderByDescending(a => a.OccurredAt).ToListAsync();
        }

        public async Task<DateTime?> GetLastActivityTimeAsync(Guid patientProfileId) =>
            await _dbSet
                .Where(a => a.PatientProfileId == patientProfileId)
                .OrderByDescending(a => a.OccurredAt)
                .Select(a => (DateTime?)a.OccurredAt)
                .FirstOrDefaultAsync();

        public async Task<double> GetAverageDailyActivityAsync(Guid patientProfileId, DateTime start, DateTime end)
        {
            var totalCount = await _dbSet.CountAsync(a =>
                a.PatientProfileId == patientProfileId &&
                a.OccurredAt >= start &&
                a.OccurredAt <= end);

            var totalDays = Math.Max(1, (end.Date - start.Date).Days + 1);

            return (double)totalCount / totalDays;
        }
    }
}