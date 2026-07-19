using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class NotificationLogRepository : GenericRepository<NotificationLog>, INotificationLogRepository
    {
        public NotificationLogRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<NotificationLog>> GetByUserIdAsync(Guid userId) =>
            await _dbSet
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
    }
}