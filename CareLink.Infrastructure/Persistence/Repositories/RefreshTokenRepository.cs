using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token) =>
            await _dbSet.SingleOrDefaultAsync(r => r.Token == token);
    }
}