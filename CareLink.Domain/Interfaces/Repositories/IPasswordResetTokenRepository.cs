using CareLink.Domain.Entities;

namespace CareLink.Domain.Interfaces.Repositories
{
    public interface IPasswordResetTokenRepository : IGenericRepository<PasswordResetToken>
    {
        Task<PasswordResetToken?> GetByTokenAsync(string token);
    }
}