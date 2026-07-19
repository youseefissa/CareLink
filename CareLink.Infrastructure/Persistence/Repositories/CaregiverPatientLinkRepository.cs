using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class CaregiverPatientLinkRepository : GenericRepository<CaregiverPatientLink>, ICaregiverPatientLinkRepository
    {
        public CaregiverPatientLinkRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<bool> LinkExistsAsync(Guid caregiverProfileId, Guid patientProfileId) =>
            await _dbSet.AnyAsync(l =>
                l.CaregiverProfileId == caregiverProfileId &&
                l.PatientProfileId == patientProfileId);
    }
}