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
        private readonly IAlertNotifier _alertNotifier;
        private readonly IAiServiceClient _aiServiceClient;

        public FallDetectionService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IAlertNotifier alertNotifier,
            IAiServiceClient aiServiceClient)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _alertNotifier = alertNotifier;
            _aiServiceClient = aiServiceClient;
        }

        public async Task<Result<FallEventDto>> RecordFallEventAsync(CreateFallEventDto request)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(request.PatientProfileId);
            if (patient is null)
                return Result<FallEventDto>.Failure("Patient profile not found.");

            if (_currentUser.Role == "Patient" && _currentUser.UserId != patient.UserId)
                return Result<FallEventDto>.Failure("You can only record fall events for your own profile.");

            var fallEvent = await CreateAndProcessFallEventAsync(request.PatientProfileId, request.IsFall, request.Confidence, request.Latitude, request.Longitude);

            return Result<FallEventDto>.Success(MapToDto(fallEvent));
        }

        public async Task<Result<FallEventDto>> AnalyzeImageAsync(Guid patientProfileId, byte[] imageBytes, string fileName)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(patientProfileId);
            if (patient is null)
                return Result<FallEventDto>.Failure("Patient profile not found.");

            if (_currentUser.Role == "Patient" && _currentUser.UserId != patient.UserId)
                return Result<FallEventDto>.Failure("You can only analyze falls for your own profile.");

            var aiResult = await _aiServiceClient.AnalyzeFallAsync(imageBytes, fileName);

            if (!aiResult.Detected)
                return Result<FallEventDto>.Failure("Could not detect a person clearly in the image.");

            var fallEvent = await CreateAndProcessFallEventAsync(patientProfileId, aiResult.IsFall, aiResult.Confidence, null, null);

            return Result<FallEventDto>.Success(MapToDto(fallEvent));
        }

        private async Task<FallEvent> CreateAndProcessFallEventAsync(
            Guid patientProfileId, bool isFall, double confidence, double? latitude, double? longitude)
        {
            var fallEvent = new FallEvent
            {
                PatientProfileId = patientProfileId,
                IsFall = isFall,
                Confidence = confidence,
                FallType = null,
                Latitude = latitude,
                Longitude = longitude,
                CaregiverNotified = false
            };

            await _unitOfWork.FallEvents.AddAsync(fallEvent);

            if (isFall)
            {
                var alert = new Alert
                {
                    PatientProfileId = patientProfileId,
                    Type = AlertType.Fall,
                    Severity = AlertSeverity.High,
                    Message = "A fall has been detected.",
                    IsResolved = false
                };

                await _unitOfWork.Alerts.AddAsync(alert);
                fallEvent.CaregiverNotified = true;

                await _alertNotifier.NotifyNewAlertAsync(new Application.DTOs.Alert.AlertBroadcastDto
                {
                    Id = alert.Id,
                    PatientProfileId = alert.PatientProfileId,
                    Type = (int)alert.Type,
                    Severity = (int)alert.Severity,
                    Message = alert.Message,
                    CreatedAt = alert.CreatedAt
                });

                var recentFallsCount = await _unitOfWork.FallEvents.CountFallsInPeriodAsync(
                    patientProfileId, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);

                if (recentFallsCount >= 3)
                {
                    var repeatedAlert = new Alert
                    {
                        PatientProfileId = patientProfileId,
                        Type = AlertType.RepeatedFalls,
                        Severity = AlertSeverity.Critical,
                        Message = "Three or more falls detected this week.",
                        IsResolved = false
                    };

                    await _unitOfWork.Alerts.AddAsync(repeatedAlert);

                    await _alertNotifier.NotifyNewAlertAsync(new Application.DTOs.Alert.AlertBroadcastDto
                    {
                        Id = repeatedAlert.Id,
                        PatientProfileId = repeatedAlert.PatientProfileId,
                        Type = (int)repeatedAlert.Type,
                        Severity = (int)repeatedAlert.Severity,
                        Message = repeatedAlert.Message,
                        CreatedAt = repeatedAlert.CreatedAt
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return fallEvent;
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