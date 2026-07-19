using CareLink.Application.DTOs.ActivityLog;
using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [Authorize(Roles = "Patient,Caregiver,Admin")]
    public class ActivityLogController : BaseApiController
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        [HttpPost("log")]
        public async Task<IActionResult> Log([FromBody] CreateActivityLogDto request)
        {
            var result = await _activityLogService.LogActivityAsync(request);
            return HandleResult(result);
        }

        [HttpGet("patient/{patientProfileId:guid}/history")]
        public async Task<IActionResult> GetHistory(Guid patientProfileId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var result = await _activityLogService.GetHistoryAsync(patientProfileId, from, to);
            return HandleResult(result);
        }
    }
}