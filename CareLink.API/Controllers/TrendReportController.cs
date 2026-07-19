using CareLink.Application.DTOs.TrendReport;
using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [Authorize(Roles = "Patient,Caregiver,Admin")]
    public class TrendReportController : BaseApiController
    {
        private readonly ITrendReportService _trendReportService;

        public TrendReportController(ITrendReportService trendReportService)
        {
            _trendReportService = trendReportService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateTrendReportDto request)
        {
            var result = await _trendReportService.GenerateAsync(request);
            return HandleResult(result);
        }

        [HttpGet("patient/{patientProfileId:guid}/history")]
        public async Task<IActionResult> GetHistory(Guid patientProfileId)
        {
            var result = await _trendReportService.GetHistoryAsync(patientProfileId);
            return HandleResult(result);
        }
    }
}