using CareLink.Application.Common;
using CareLink.Application.DTOs.SafetyRecommendation;

namespace CareLink.Application.Interfaces
{
    public interface ISafetyRecommendationService
    {
        Task<Result<IReadOnlyList<SafetyRecommendationDto>>> GetForPatientAsync(Guid patientProfileId);
        Task<Result> AcknowledgeAsync(Guid recommendationId);
    }
}