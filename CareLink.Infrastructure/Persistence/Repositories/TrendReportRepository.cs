using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class TrendReportRepository : GenericRepository<TrendReport>, ITrendReportRepository
    {
        public TrendReportRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<TrendReport>> GetByPatientIdAsync(Guid patientProfileId) =>
            await _dbSet
                .Where(r => r.PatientProfileId == patientProfileId)
                .OrderByDescending(r => r.PeriodEnd)
                .ToListAsync();
    }
}