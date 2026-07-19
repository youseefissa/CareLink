namespace CareLink.Application.DTOs.Caregiver
{
    public class CreateCaregiverProfileDto
    {
        public Guid UserId { get; set; }
        public string RelationshipType { get; set; } = string.Empty;
    }
}