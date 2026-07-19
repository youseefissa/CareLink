using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [Authorize(Roles = "Patient,Caregiver,Admin")]
    public class SafetyRecommendationController : BaseApiController
    {
        private readonly ISafetyRecommendationService _safetyRecommendationService;

        public SafetyRecommendationController(ISafetyRecommendationService safetyRecommendationService)
        {
            _safetyRecommendationService = safetyRecommendationService;
        }

        [HttpGet("patient/{patientProfileId:guid}")]
        public async Task<IActionResult> GetForPatient(Guid patientProfileId)
        {
            var result = await _safetyRecommendationService.GetForPatientAsync(patientProfileId);
            return HandleResult(result);
        }

        [HttpPost("{recommendationId:guid}/acknowledge")]
        public async Task<IActionResult> Acknowledge(Guid recommendationId)
        {
            var result = await _safetyRecommendationService.AcknowledgeAsync(recommendationId);
            return HandleResult(result);
        }
    }
}