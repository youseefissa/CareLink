using CareLink.Application.Common;
using CareLink.Application.DTOs.Patient;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public PatientService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<PatientProfileDto>> CreateProfileAsync(CreatePatientProfileDto request)
        {
            if (_currentUser.Role == "Patient" && _currentUser.UserId != request.UserId)
                return Result<PatientProfileDto>.Failure("You can only create a profile for your own account.");

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user is null)
                return Result<PatientProfileDto>.Failure("User not found.");

            var existingProfile = await _unitOfWork.PatientProfiles.GetByUserIdAsync(request.UserId);
            if (existingProfile is not null)
                return Result<PatientProfileDto>.Failure("Patient profile already exists for this user.");

            var profile = new PatientProfile
            {
                UserId = request.UserId,
                DateOfBirth = request.DateOfBirth,
                MedicalNotes = request.MedicalNotes,
                HasVisualImpairment = request.HasVisualImpairment,
                HasHearingImpairment = request.HasHearingImpairment,
                EmergencyContactPhone = request.EmergencyContactPhone
            };

            if (request.SleepWindowStart.HasValue)
                profile.SleepWindowStart = request.SleepWindowStart.Value;

            if (request.SleepWindowEnd.HasValue)
                profile.SleepWindowEnd = request.SleepWindowEnd.Value;

            await _unitOfWork.PatientProfiles.AddAsync(profile);
            await _unitOfWork.SaveChangesAsync();

            return Result<PatientProfileDto>.Success(MapToDto(profile, user.FullName));
        }

        public async Task<Result<PatientProfileDto>> GetByIdAsync(Guid patientProfileId)
        {
            var profile = await _unitOfWork.PatientProfiles.GetWithDetailsAsync(patientProfileId);
            if (profile is null)
                return Result<PatientProfileDto>.Failure("Patient profile not found.");

            var canAccess = await CanAccessPatientAsync(profile);
            if (!canAccess)
                return Result<PatientProfileDto>.Failure("You do not have permission to view this profile.");

            return Result<PatientProfileDto>.Success(MapToDto(profile, profile.User.FullName));
        }

        public async Task<Result<PatientProfileDto>> UpdateProfileAsync(Guid patientProfileId, UpdatePatientProfileDto request)
        {
            var profile = await _unitOfWork.PatientProfiles.GetWithDetailsAsync(patientProfileId);
            if (profile is null)
                return Result<PatientProfileDto>.Failure("Patient profile not found.");

            var isOwner = _currentUser.UserId == profile.UserId;
            var isAdmin = _currentUser.Role == "Admin";

            if (!isOwner && !isAdmin)
                return Result<PatientProfileDto>.Failure("Only the patient or an admin can update this profile.");

            profile.MedicalNotes = request.MedicalNotes;
            profile.HasVisualImpairment = request.HasVisualImpairment;
            profile.HasHearingImpairment = request.HasHearingImpairment;
            profile.EmergencyContactPhone = request.EmergencyContactPhone;

            if (request.SleepWindowStart.HasValue)
                profile.SleepWindowStart = request.SleepWindowStart.Value;

            if (request.SleepWindowEnd.HasValue)
                profile.SleepWindowEnd = request.SleepWindowEnd.Value;

            _unitOfWork.PatientProfiles.Update(profile);
            await _unitOfWork.SaveChangesAsync();

            return Result<PatientProfileDto>.Success(MapToDto(profile, profile.User.FullName));
        }

        private async Task<bool> CanAccessPatientAsync(PatientProfile profile)
        {
            if (_currentUser.Role == "Admin")
                return true;

            if (_currentUser.UserId == profile.UserId)
                return true;

            if (_currentUser.Role == "Caregiver" && _currentUser.UserId.HasValue)
            {
                var caregiverProfile = await _unitOfWork.CaregiverProfiles.GetByUserIdAsync(_currentUser.UserId.Value);
                if (caregiverProfile is null)
                    return false;

                return await _unitOfWork.CaregiverPatientLinks.LinkExistsAsync(caregiverProfile.Id, profile.Id);
            }

            return false;
        }

        private static PatientProfileDto MapToDto(PatientProfile profile, string fullName) => new()
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FullName = fullName,
            DateOfBirth = profile.DateOfBirth,
            MedicalNotes = profile.MedicalNotes,
            HasVisualImpairment = profile.HasVisualImpairment,
            HasHearingImpairment = profile.HasHearingImpairment,
            EmergencyContactPhone = profile.EmergencyContactPhone,
            SleepWindowStart = profile.SleepWindowStart,
            SleepWindowEnd = profile.SleepWindowEnd
        };
    }
}