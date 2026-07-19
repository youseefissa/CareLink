using CareLink.Application.Common;
using CareLink.Application.DTOs.Fall;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Enums;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class FallDetectionService : IFallDetectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public FallDetectionService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<FallEventDto>> RecordFallEventAsync(CreateFallEventDto request)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(request.PatientProfileId);
            if (patient is null)
                return Result<FallEventDto>.Failure("Patient profile not found.");

            if (_currentUser.Role == "Patient" && _currentUser.UserId != patient.UserId)
                return Result<FallEventDto>.Failure("You can only record fall events for your own profile.");

            var fallEvent = new FallEvent
            {
                PatientProfileId = request.PatientProfileId,
                IsFall = request.IsFall,
                Confidence = request.Confidence,
                FallType = null,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                CaregiverNotified = false
            };

            await _unitOfWork.FallEvents.AddAsync(fallEvent);

            if (request.IsFall)
            {
                var alert = new Alert
                {
                    PatientProfileId = request.PatientProfileId,
                    Type = AlertType.Fall,
                    Severity = AlertSeverity.High,
                    Message = "A fall has been detected.",
                    IsResolved = false
                };

                await _unitOfWork.Alerts.AddAsync(alert);
                fallEvent.CaregiverNotified = true;

                var recentFallsCount = await _unitOfWork.FallEvents.CountFallsInPeriodAsync(
                    request.PatientProfileId, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);

                if (recentFallsCount >= 3)
                {
                    var repeatedAlert = new Alert
                    {
                        PatientProfileId = request.PatientProfileId,
                        Type = AlertType.RepeatedFalls,
                        Severity = AlertSeverity.Critical,
                        Message = "Three or more falls detected this week.",
                        IsResolved = false
                    };

                    await _unitOfWork.Alerts.AddAsync(repeatedAlert);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return Result<FallEventDto>.Success(MapToDto(fallEvent));
        }

        public async Task<Result<IReadOnlyList<FallEventDto>>> GetHistoryAsync(Guid patientProfileId)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(patientProfileId);
            if (patient is null)
                return Result<IReadOnlyList<FallEventDto>>.Failure("Patient profile not found.");

            var canAccess = await CanAccessPatientAsync(patient);
            if (!canAccess)
                return Result<IReadOnlyList<FallEventDto>>.Failure("You do not have permission to view this history.");

            var events = await _unitOfWork.FallEvents.GetByPatientIdAsync(patientProfileId);

            var dtoList = events.Select(MapToDto).ToList();

            return Result<IReadOnlyList<FallEventDto>>.Success(dtoList);
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

        private static FallEventDto MapToDto(FallEvent fallEvent) => new()
        {
            Id = fallEvent.Id,
            PatientProfileId = fallEvent.PatientProfileId,
            IsFall = fallEvent.IsFall,
            Confidence = fallEvent.Confidence,
            FallType = fallEvent.FallType,
            Latitude = fallEvent.Latitude,
            Longitude = fallEvent.Longitude,
            CreatedAt = fallEvent.CreatedAt
        };
    }
}