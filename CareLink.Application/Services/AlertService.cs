using CareLink.Application.Common;
using CareLink.Application.DTOs.Alert;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public AlertService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<IReadOnlyList<AlertDto>>> GetForPatientAsync(Guid patientProfileId)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(patientProfileId);
            if (patient is null)
                return Result<IReadOnlyList<AlertDto>>.Failure("Patient profile not found.");

            var canAccess = await CanAccessPatientAsync(patient);
            if (!canAccess)
                return Result<IReadOnlyList<AlertDto>>.Failure("You do not have permission to view these alerts.");

            var alerts = await _unitOfWork.Alerts.GetByPatientIdAsync(patientProfileId);

            var dtoList = alerts.Select(a => new AlertDto
            {
                Id = a.Id,
                Type = a.Type,
                Severity = a.Severity,
                Message = a.Message,
                IsResolved = a.IsResolved,
                CreatedAt = a.CreatedAt
            }).ToList();

            return Result<IReadOnlyList<AlertDto>>.Success(dtoList);
        }

        public async Task<Result> ResolveAsync(Guid alertId)
        {
            var alert = await _unitOfWork.Alerts.GetByIdAsync(alertId);
            if (alert is null)
                return Result.Failure("Alert not found.");

            if (_currentUser.Role == "Patient")
                return Result.Failure("Only a caregiver or admin can resolve an alert.");

            alert.IsResolved = true;
            alert.ResolvedAt = DateTime.UtcNow;

            _unitOfWork.Alerts.Update(alert);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
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
    }
}