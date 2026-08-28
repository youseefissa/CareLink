using CareLink.Application.DTOs.Patient;
using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [Authorize(Roles = "Patient,Caregiver,Admin")]
   
    public class PatientController : BaseApiController
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpPost("profile")]
        public async Task<IActionResult> CreateProfile([FromBody] CreatePatientProfileDto request)
        {
            var result = await _patientService.CreateProfileAsync(request);
            return HandleResult(result);
        }

        [HttpGet("profile/{patientProfileId:guid}")]
        public async Task<IActionResult> GetProfile(Guid patientProfileId)
        {
            var result = await _patientService.GetByIdAsync(patientProfileId);
            return HandleResult(result);
        }

        [HttpPut("profile/{patientProfileId:guid}")]
        public async Task<IActionResult> UpdateProfile(Guid patientProfileId, [FromBody] UpdatePatientProfileDto request)
        {
            var result = await _patientService.UpdateProfileAsync(patientProfileId, request);
            return HandleResult(result);
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _patientService.GetMyProfileAsync();
            return HandleResult(result);
        }
    }

}
