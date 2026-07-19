using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class SOSEventRepository : GenericRepository<SOSEvent>, ISOSEventRepository
    {
        public SOSEventRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<SOSEvent>> GetByPatientIdAsync(Guid patientProfileId) =>
            await _dbSet
                .Where(s => s.PatientProfileId == patientProfileId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

        public async Task<IReadOnlyList<SOSEvent>> GetUnresolvedAsync(Guid patientProfileId) =>
            await _dbSet
                .Where(s => s.PatientProfileId == patientProfileId && !s.Resolved)
                .ToListAsync();
    }
}