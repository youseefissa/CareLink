using CareLink.Application.Common;
using CareLink.Application.DTOs.SOS;
using CareLink.Application.DTOs.Voice;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class VoiceCommandService : IVoiceCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISOSService _sosService;
        private readonly ICurrentUserService _currentUser;

        private static readonly Dictionary<string, string> CommandKeywords = new()
        {
            { "help", "SOS" },
            { "emergency", "SOS" },
            { "ساعدني", "SOS" },
            { "طوارئ", "SOS" },
            { "caregiver", "CallCaregiver" },
            { "call", "CallCaregiver" },
            { "اتصل", "CallCaregiver" },
            { "medicine", "MedicineReminder" },
            { "دواء", "MedicineReminder" }
        };

        public VoiceCommandService(IUnitOfWork unitOfWork, ISOSService sosService, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _sosService = sosService;
            _currentUser = currentUser;
        }

        public async Task<Result> ProcessCommandAsync(VoiceCommandDto request)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(request.PatientProfileId);
            if (patient is null)
                return Result.Failure("Patient profile not found.");

            if (_currentUser.Role == "Patient" && _currentUser.UserId != patient.UserId)
                return Result.Failure("You can only send voice commands for your own profile.");

            var matchedCommand = MatchCommand(request.RecognizedText);
            var wasExecuted = false;

            if (matchedCommand == "SOS")
            {
                var sosResult = await _sosService.TriggerAsync(new CreateSOSEventDto
                {
                    PatientProfileId = request.PatientProfileId,
                    TriggerSource = "Voice"
                });

                wasExecuted = sosResult.Succeeded;
            }

            var log = new VoiceCommandLog
            {
                PatientProfileId = request.PatientProfileId,
                RecognizedText = request.RecognizedText,
                MatchedCommand = matchedCommand ?? "Unknown",
                WasExecuted = wasExecuted
            };

            await _unitOfWork.VoiceCommandLogs.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        private static string? MatchCommand(string recognizedText)
        {
            var lowerText = recognizedText.ToLowerInvariant();

            foreach (var keyword in CommandKeywords)
            {
                if (lowerText.Contains(keyword.Key))
                    return keyword.Value;
            }

            return null;
        }
    }
}