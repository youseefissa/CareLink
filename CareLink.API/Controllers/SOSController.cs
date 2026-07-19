using CareLink.Application.DTOs.SOS;
using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [Authorize(Roles = "Patient,Caregiver,Admin")]
    public class SOSController : BaseApiController
    {
        private readonly ISOSService _sosService;

        public SOSController(ISOSService sosService)
        {
            _sosService = sosService;
        }

        [HttpPost("trigger")]
        public async Task<IActionResult> Trigger([FromBody] CreateSOSEventDto request)
        {
            var result = await _sosService.TriggerAsync(request);
            return HandleResult(result);
        }

        [HttpPost("{sosEventId:guid}/resolve")]
        public async Task<IActionResult> Resolve(Guid sosEventId)
        {
            var result = await _sosService.ResolveAsync(sosEventId);
            return HandleResult(result);
        }

        [HttpGet("patient/{patientProfileId:guid}/history")]
        public async Task<IActionResult> GetHistory(Guid patientProfileId)
        {
            var result = await _sosService.GetHistoryAsync(patientProfileId);
            return HandleResult(result);
        }
    }
}