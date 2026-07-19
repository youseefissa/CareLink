using CareLink.Domain.Entities;

namespace CareLink.Domain.Interfaces.Repositories
{
    public interface INotificationLogRepository : IGenericRepository<NotificationLog>
    {
        Task<IReadOnlyList<NotificationLog>> GetByUserIdAsync(Guid userId);
    }
}