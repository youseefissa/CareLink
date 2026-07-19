using CareLink.Application.Common;
using CareLink.Application.DTOs.SOS;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Enums;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class SOSService : ISOSService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public SOSService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<SOSEventDto>> TriggerAsync(CreateSOSEventDto request)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(request.PatientProfileId);
            if (patient is null)
                return Result<SOSEventDto>.Failure("Patient profile not found.");

            if (_currentUser.Role == "Patient" && _currentUser.UserId != patient.UserId)
                return Result<SOSEventDto>.Failure("You can only trigger SOS for your own profile.");

            var sosEvent = new SOSEvent
            {
                PatientProfileId = request.PatientProfileId,
                TriggerSource = request.TriggerSource,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Resolved = false
            };

            await _unitOfWork.SOSEvents.AddAsync(sosEvent);

            var alert = new Alert
            {
                PatientProfileId = request.PatientProfileId,
                Type = AlertType.SOS,
                Severity = AlertSeverity.Critical,
                Message = "Emergency button pressed.",
                IsResolved = false
            };

            await _unitOfWork.Alerts.AddAsync(alert);

            await _unitOfWork.SaveChangesAsync();

            return Result<SOSEventDto>.Success(MapToDto(sosEvent));
        }

        public async Task<Result> ResolveAsync(Guid sosEventId)
        {
            var sosEvent = await _unitOfWork.SOSEvents.GetByIdAsync(sosEventId);
            if (sosEvent is null)
                return Result.Failure("SOS event not found.");

            if (_currentUser.Role == "Patient")
                return Result.Failure("Only a caregiver or admin can resolve an SOS event.");

            sosEvent.Resolved = true;
            sosEvent.ResolvedAt = DateTime.UtcNow;

            _unitOfWork.SOSEvents.Update(sosEvent);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<IReadOnlyList<SOSEventDto>>> GetHistoryAsync(Guid patientProfileId)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(patientProfileId);
            if (patient is null)
                return Result<IReadOnlyList<SOSEventDto>>.Failure("Patient profile not found.");

            var canAccess = await CanAccessPatientAsync(patient);
            if (!canAccess)
                return Result<IReadOnlyList<SOSEventDto>>.Failure("You do not have permission to view this history.");

            var events = await _unitOfWork.SOSEvents.GetByPatientIdAsync(patientProfileId);

            var dtoList = events.Select(MapToDto).ToList();

            return Result<IReadOnlyList<SOSEventDto>>.Success(dtoList);
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

        private static SOSEventDto MapToDto(SOSEvent sosEvent) => new()
        {
            Id = sosEvent.Id,
            PatientProfileId = sosEvent.PatientProfileId,
            TriggerSource = sosEvent.TriggerSource,
            Latitude = sosEvent.Latitude,
            Longitude = sosEvent.Longitude,
            Resolved = sosEvent.Resolved,
            CreatedAt = sosEvent.CreatedAt
        };
    }
}