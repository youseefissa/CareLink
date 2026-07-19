using CareLink.Domain.Entities;

namespace CareLink.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
    }
}