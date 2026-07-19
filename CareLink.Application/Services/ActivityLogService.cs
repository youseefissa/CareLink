using CareLink.Application.Common;
using CareLink.Application.DTOs.ActivityLog;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public ActivityLogService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<ActivityLogDto>> LogActivityAsync(CreateActivityLogDto request)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(request.PatientProfileId);
            if (patient is null)
                return Result<ActivityLogDto>.Failure("Patient profile not found.");

            if (_currentUser.Role == "Patient" && _currentUser.UserId != patient.UserId)
                return Result<ActivityLogDto>.Failure("You can only log activity for your own profile.");

            var log = new ActivityLog
            {
                PatientProfileId = request.PatientProfileId,
                ActivityType = request.ActivityType,
                Details = request.Details,
                OccurredAt = DateTime.UtcNow
            };

            await _unitOfWork.ActivityLogs.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();

            return Result<ActivityLogDto>.Success(MapToDto(log));
        }

        public async Task<Result<IReadOnlyList<ActivityLogDto>>> GetHistoryAsync(Guid patientProfileId, DateTime? from = null, DateTime? to = null)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(patientProfileId);
            if (patient is null)
                return Result<IReadOnlyList<ActivityLogDto>>.Failure("Patient profile not found.");

            var canAccess = await CanAccessPatientAsync(patient);
            if (!canAccess)
                return Result<IReadOnlyList<ActivityLogDto>>.Failure("You do not have permission to view this history.");

            var logs = await _unitOfWork.ActivityLogs.GetByPatientIdAsync(patientProfileId, from, to);

            var dtoList = logs.Select(MapToDto).ToList();

            return Result<IReadOnlyList<ActivityLogDto>>.Success(dtoList);
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

        private static ActivityLogDto MapToDto(ActivityLog log) => new()
        {
            Id = log.Id,
            ActivityType = log.ActivityType,
            Details = log.Details,
            OccurredAt = log.OccurredAt
        };
    }
}