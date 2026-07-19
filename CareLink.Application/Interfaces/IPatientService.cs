using CareLink.Application.Common;
using CareLink.Application.DTOs.Patient;

namespace CareLink.Application.Interfaces
{
    public interface IPatientService
    {
        Task<Result<PatientProfileDto>> CreateProfileAsync(CreatePatientProfileDto request);
        Task<Result<PatientProfileDto>> GetByIdAsync(Guid patientProfileId);
        Task<Result<PatientProfileDto>> UpdateProfileAsync(Guid patientProfileId, UpdatePatientProfileDto request);
    }
}