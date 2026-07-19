using CareLink.Application.DTOs.Fall;
using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [Authorize(Roles = "Patient,Caregiver,Admin")]

    public class FallController : BaseApiController
    {
        private readonly IFallDetectionService _fallDetectionService;

        public FallController(IFallDetectionService fallDetectionService)
        {
            _fallDetectionService = fallDetectionService;
        }

        [HttpPost("record")]
        public async Task<IActionResult> Record([FromBody] CreateFallEventDto request)
        {
            var result = await _fallDetectionService.RecordFallEventAsync(request);
            return HandleResult(result);
        }

        [HttpGet("patient/{patientProfileId:guid}/history")]
        public async Task<IActionResult> GetHistory(Guid patientProfileId)
        {
            var result = await _fallDetectionService.GetHistoryAsync(patientProfileId);
            return HandleResult(result);
        }
    }
}