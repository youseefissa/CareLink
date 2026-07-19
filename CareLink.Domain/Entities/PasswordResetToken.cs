using CareLink.Domain.Entities.Common;

namespace CareLink.Domain.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }

        public bool IsActive => !IsUsed && ExpiresAt > DateTime.UtcNow;
    }
}