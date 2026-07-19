using CareLink.Application.DTOs.Caregiver;
using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [Authorize(Roles = "Caregiver,Admin")]
    public class CaregiverController : BaseApiController
    {
        private readonly ICaregiverService _caregiverService;
        private readonly ICaregiverDashboardService _dashboardService;

        public CaregiverController(ICaregiverService caregiverService, ICaregiverDashboardService dashboardService)
        {
            _caregiverService = caregiverService;
            _dashboardService = dashboardService;
        }

        [HttpPost("profile")]
        public async Task<IActionResult> CreateProfile([FromBody] CreateCaregiverProfileDto request)
        {
            var result = await _caregiverService.CreateProfileAsync(request);
            return HandleResult(result);
        }

        [HttpGet("profile/{caregiverProfileId:guid}")]
        public async Task<IActionResult> GetProfile(Guid caregiverProfileId)
        {
            var result = await _caregiverService.GetByIdAsync(caregiverProfileId);
            return HandleResult(result);
        }

        [HttpPost("link-patient")]
        public async Task<IActionResult> LinkPatient([FromBody] LinkCaregiverPatientDto request)
        {
            var result = await _dashboardService.LinkPatientAsync(request);
            return HandleResult(result);
        }

        [HttpGet("{caregiverProfileId:guid}/dashboard")]
        public async Task<IActionResult> GetDashboard(Guid caregiverProfileId)
        {
            var result = await _dashboardService.GetDashboardAsync(caregiverProfileId);
            return HandleResult(result);
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _caregiverService.GetMyProfileAsync();
            return HandleResult(result);
        }
    }
}