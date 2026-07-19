using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [Authorize(Roles = "Patient,Caregiver,Admin")]
    public class AlertController : BaseApiController
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpGet("patient/{patientProfileId:guid}")]
        public async Task<IActionResult> GetForPatient(Guid patientProfileId)
        {
            var result = await _alertService.GetForPatientAsync(patientProfileId);
            return HandleResult(result);
        }

        [HttpPost("{alertId:guid}/resolve")]
        public async Task<IActionResult> Resolve(Guid alertId)
        {
            var result = await _alertService.ResolveAsync(alertId);
            return HandleResult(result);
        }
    }
}