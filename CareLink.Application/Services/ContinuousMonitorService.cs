using CareLink.Application.Common;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Enums;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class ContinuousMonitorService : IContinuousMonitorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IAlertNotifier _alertNotifier;
        private readonly IAiServiceClient _aiServiceClient;

        public ContinuousMonitorService(
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

        public async Task<Result> ProcessFrameAsync(Guid patientProfileId, byte[] imageBytes, string fileName)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(patientProfileId);
            if (patient is null)
                return Result.Failure("Patient profile not found.");

            if (_currentUser.Role == "Patient" && _currentUser.UserId != patient.UserId)
                return Result.Failure("You can only monitor your own profile.");

            var aiResult = await _aiServiceClient.AnalyzeContinuousFrameAsync(patientProfileId, imageBytes, fileName);

            if (!aiResult.Detected || !aiResult.Emergency)
                return Result.Success();

            var existingUnresolvedAlerts = await _unitOfWork.Alerts.GetUnresolvedAsync(patientProfileId);
            var alreadyHasSimilarAlert = existingUnresolvedAlerts.Any(a =>
                a.Type == AlertType.Fall || a.Type == AlertType.NoMovement);

            if (alreadyHasSimilarAlert)
                return Result.Success();

            var alertType = aiResult.IsFall ? AlertType.Fall : AlertType.NoMovement;
            var severity = aiResult.IsFall ? AlertSeverity.Critical : AlertSeverity.High;

            var alert = new Alert
            {
                PatientProfileId = patientProfileId,
                Type = alertType,
                Severity = severity,
                Message = aiResult.Reason ?? "Possible emergency detected by continuous monitoring.",
                IsResolved = false
            };

            await _unitOfWork.Alerts.AddAsync(alert);
            await _unitOfWork.SaveChangesAsync();

            await _alertNotifier.NotifyNewAlertAsync(new Application.DTOs.Alert.AlertBroadcastDto
            {
                Id = alert.Id,
                PatientProfileId = alert.PatientProfileId,
                Type = (int)alert.Type,
                Severity = (int)alert.Severity,
                Message = alert.Message,
                CreatedAt = alert.CreatedAt
            });

            return Result.Success();
        }
    }
}