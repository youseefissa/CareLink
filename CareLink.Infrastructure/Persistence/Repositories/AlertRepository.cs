using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class AlertRepository : GenericRepository<Alert>, IAlertRepository
    {
        public AlertRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Alert>> GetByPatientIdAsync(Guid patientProfileId) =>
            await _dbSet
                .Where(a => a.PatientProfileId == patientProfileId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task<IReadOnlyList<Alert>> GetUnresolvedAsync(Guid patientProfileId) =>
            await _dbSet
                .Where(a => a.PatientProfileId == patientProfileId && !a.IsResolved)
                .ToListAsync();
    }
}