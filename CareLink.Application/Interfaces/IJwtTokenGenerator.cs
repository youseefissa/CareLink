using CareLink.Domain.Entities;

namespace CareLink.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime ExpiresAt) GenerateToken(User user);
        string GenerateRefreshToken();
    }
}