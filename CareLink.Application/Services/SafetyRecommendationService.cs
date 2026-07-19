using CareLink.Application.Common;
using CareLink.Application.DTOs.SafetyRecommendation;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class SafetyRecommendationService : ISafetyRecommendationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public SafetyRecommendationService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<IReadOnlyList<SafetyRecommendationDto>>> GetForPatientAsync(Guid patientProfileId)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(patientProfileId);
            if (patient is null)
                return Result<IReadOnlyList<SafetyRecommendationDto>>.Failure("Patient profile not found.");

            var canAccess = await CanAccessPatientAsync(patient);
            if (!canAccess)
                return Result<IReadOnlyList<SafetyRecommendationDto>>.Failure("You do not have permission to view these recommendations.");

            await GenerateRecommendationsIfNeededAsync(patientProfileId);

            var recommendations = await _unitOfWork.SafetyRecommendations.GetByPatientIdAsync(patientProfileId);

            var dtoList = recommendations.Select(r => new SafetyRecommendationDto
            {
                Id = r.Id,
                RecommendationText = r.RecommendationText,
                Category = r.Category,
                IsAcknowledged = r.IsAcknowledged,
                CreatedAt = r.CreatedAt
            }).ToList();

            return Result<IReadOnlyList<SafetyRecommendationDto>>.Success(dtoList);
        }

        public async Task<Result> AcknowledgeAsync(Guid recommendationId)
        {
            var recommendation = await _unitOfWork.SafetyRecommendations.GetByIdAsync(recommendationId);
            if (recommendation is null)
                return Result.Failure("Safety recommendation not found.");

            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(recommendation.PatientProfileId);
            if (patient is null)
                return Result.Failure("Patient profile not found.");

            if (_currentUser.Role == "Patient" && _currentUser.UserId != patient.UserId)
                return Result.Failure("You can only acknowledge your own recommendations.");

            recommendation.IsAcknowledged = true;

            _unitOfWork.SafetyRecommendations.Update(recommendation);
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

        private async Task GenerateRecommendationsIfNeededAsync(Guid patientProfileId)
        {
            var weekStart = DateTime.UtcNow.AddDays(-7);
            var fallsThisWeek = await _unitOfWork.FallEvents.CountFallsInPeriodAsync(patientProfileId, weekStart, DateTime.UtcNow);

            var existingRecommendations = await _unitOfWork.SafetyRecommendations.GetByPatientIdAsync(patientProfileId);
            var hasUnacknowledgedMobilityTip = existingRecommendations
                .Any(r => r.Category == "Mobility" && !r.IsAcknowledged);

            if (fallsThisWeek >= 2 && !hasUnacknowledgedMobilityTip)
            {
                await _unitOfWork.SafetyRecommendations.AddAsync(new SafetyRecommendation
                {
                    PatientProfileId = patientProfileId,
                    RecommendationText = "Improve hallway lighting and remove loose rugs to reduce fall risk.",
                    Category = "Mobility",
                    IsAcknowledged = false
                });
            }

            var lastActivity = await _unitOfWork.ActivityLogs.GetLastActivityTimeAsync(patientProfileId);
            var isInactiveOverADay = lastActivity is null || (DateTime.UtcNow - lastActivity.Value) >= TimeSpan.FromHours(24);

            var hasUnacknowledgedActivityTip = existingRecommendations
                .Any(r => r.Category == "General" && !r.IsAcknowledged);

            if (isInactiveOverADay && !hasUnacknowledgedActivityTip)
            {
                await _unitOfWork.SafetyRecommendations.AddAsync(new SafetyRecommendation
                {
                    PatientProfileId = patientProfileId,
                    RecommendationText = "Please confirm you have taken your medication and try to stay active today.",
                    Category = "General",
                    IsAcknowledged = false
                });
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}