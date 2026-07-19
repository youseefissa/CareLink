using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class PasswordResetTokenRepository : GenericRepository<PasswordResetToken>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<PasswordResetToken?> GetByTokenAsync(string token) =>
            await _dbSet.SingleOrDefaultAsync(t => t.Token == token);
    }
}