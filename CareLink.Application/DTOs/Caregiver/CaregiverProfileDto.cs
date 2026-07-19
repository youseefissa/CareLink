namespace CareLink.Application.DTOs.Caregiver
{
    public class CaregiverProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string RelationshipType { get; set; } = string.Empty;
    }
}