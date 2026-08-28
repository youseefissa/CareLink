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
        private readonly IAiServiceClient _aiServiceClient;

        private const double MinimumConfidenceThreshold = 0.75;

        public GestureCommandService(
            IUnitOfWork unitOfWork,
            ISOSService sosService,
            ICurrentUserService currentUser,
            IAiServiceClient aiServiceClient)
        {
            _unitOfWork = unitOfWork;
            _sosService = sosService;
            _currentUser = currentUser;
            _aiServiceClient = aiServiceClient;
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

        public async Task<Result<AnalyzeGestureResultDto>> AnalyzeImageAsync(Guid patientProfileId, byte[] imageBytes, string fileName)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(patientProfileId);
            if (patient is null)
                return Result<AnalyzeGestureResultDto>.Failure("Patient profile not found.");

            if (_currentUser.Role == "Patient" && _currentUser.UserId != patient.UserId)
                return Result<AnalyzeGestureResultDto>.Failure("You can only analyze gestures for your own profile.");

            var aiResult = await _aiServiceClient.AnalyzeHandGestureAsync(imageBytes, fileName);

            var wasExecuted = false;
            GestureType? gestureType = aiResult.Gesture switch
            {
                "OpenPalm" => GestureType.OpenPalm,
                "ClosedFist" => GestureType.ClosedFist,
                "Victory" => GestureType.Victory,
                _ => null
            };

            if (aiResult.Detected && gestureType.HasValue && aiResult.Confidence >= MinimumConfidenceThreshold)
            {
                if (gestureType == GestureType.OpenPalm)
                {
                    var sosResult = await _sosService.TriggerAsync(new CreateSOSEventDto
                    {
                        PatientProfileId = patientProfileId,
                        TriggerSource = "Gesture"
                    });

                    wasExecuted = sosResult.Succeeded;
                }
                else
                {
                    wasExecuted = true;
                }

                var log = new GestureCommandLog
                {
                    PatientProfileId = patientProfileId,
                    Gesture = gestureType.Value,
                    Confidence = aiResult.Confidence,
                    WasExecuted = wasExecuted
                };

                await _unitOfWork.GestureCommandLogs.AddAsync(log);
                await _unitOfWork.SaveChangesAsync();
            }

            return Result<AnalyzeGestureResultDto>.Success(new AnalyzeGestureResultDto
            {
                Gesture = aiResult.Gesture,
                Confidence = aiResult.Confidence,
                Detected = aiResult.Detected,
                WasExecuted = wasExecuted
            });
        }
    }
}