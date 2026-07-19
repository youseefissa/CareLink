using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class VoiceCommandLogRepository : GenericRepository<VoiceCommandLog>, IVoiceCommandLogRepository
    {
        public VoiceCommandLogRepository(CareLinkDbContext context) : base(context)
        {
        }
    }
}