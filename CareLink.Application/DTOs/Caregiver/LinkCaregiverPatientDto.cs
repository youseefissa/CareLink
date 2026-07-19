namespace CareLink.Application.DTOs.Caregiver
{
    public class LinkCaregiverPatientDto
    {
        public Guid CaregiverProfileId { get; set; }
        public Guid PatientProfileId { get; set; }
        public bool IsPrimaryCaregiver { get; set; }
    }
}