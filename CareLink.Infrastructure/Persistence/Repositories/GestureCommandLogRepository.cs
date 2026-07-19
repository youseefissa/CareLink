using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class GestureCommandLogRepository : GenericRepository<GestureCommandLog>, IGestureCommandLogRepository
    {
        public GestureCommandLogRepository(CareLinkDbContext context) : base(context)
        {
        }
    }
}