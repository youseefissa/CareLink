using CareLink.Application.Common;
using CareLink.Application.DTOs.Gesture;
using CareLink.Application.DTOs.SOS;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Enums;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class GestureCommandService : IGestureCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISOSService _sosService;
        private readonly ICurrentUserService _currentUser;

        private const double MinimumConfidenceThreshold = 0.75;

        public GestureCommandService(IUnitOfWork unitOfWork, ISOSService sosService, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _sosService = sosService;
            _currentUser = currentUser;
        }

        public async Task<Result> ProcessGestureAsync(GestureCommandDto request)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(request.PatientProfileId);
            if (patient is null)
                return Result.Failure("Patient profile not found.");

            if (_currentUser.Role == "Patient" && _currentUser.UserId != patient.UserId)
                return Result.Failure("You can only send gesture commands for your own profile.");

            var wasExecuted = false;

            if (request.Confidence >= MinimumConfidenceThreshold)
            {
                if (request.Gesture == GestureType.OpenPalm)
                {
                    var sosResult = await _sosService.TriggerAsync(new CreateSOSEventDto
                    {
                        PatientProfileId = request.PatientProfileId,
                        TriggerSource = "Gesture"
                    });

                    wasExecuted = sosResult.Succeeded;
                }
                else if (request.Gesture == GestureType.ClosedFist || request.Gesture == GestureType.Victory)
                {
                    wasExecuted = true;
                }
            }

            var log = new GestureCommandLog
            {
                PatientProfileId = request.PatientProfileId,
                Gesture = request.Gesture,
                Confidence = request.Confidence,
                WasExecuted = wasExecuted
            };

            await _unitOfWork.GestureCommandLogs.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}