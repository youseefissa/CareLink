using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(CareLinkDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _dbSet.SingleOrDefaultAsync(u => u.Email == email);

        public async Task<bool> EmailExistsAsync(string email) =>
            await _dbSet.AnyAsync(u => u.Email == email);
    }
}