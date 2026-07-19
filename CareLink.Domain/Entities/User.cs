using CareLink.Domain.Entities.Common;
using CareLink.Domain.Enums;

namespace CareLink.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public string? FcmDeviceToken { get; set; }

        public PatientProfile? PatientProfile { get; set; }
        public CaregiverProfile? CaregiverProfile { get; set; }
    }
}