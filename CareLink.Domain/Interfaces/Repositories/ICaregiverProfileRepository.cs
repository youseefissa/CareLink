using CareLink.Domain.Entities;

namespace CareLink.Domain.Interfaces.Repositories
{
    public interface ICaregiverProfileRepository : IGenericRepository<CaregiverProfile>
    {
        Task<CaregiverProfile?> GetByUserIdAsync(Guid userId);
        Task<CaregiverProfile?> GetWithLinksAsync(Guid caregiverProfileId);
    }
}