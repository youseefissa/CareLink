using CareLink.Application.DTOs.Gesture;
using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [Authorize(Roles = "Patient,Caregiver,Admin")]
    public class GestureCommandController : BaseApiController
    {
        private readonly IGestureCommandService _gestureCommandService;

        public GestureCommandController(IGestureCommandService gestureCommandService)
        {
            _gestureCommandService = gestureCommandService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] GestureCommandDto request)
        {
            var result = await _gestureCommandService.ProcessGestureAsync(request);
            return HandleResult(result);
        }

        [HttpPost("analyze-image")]
        public async Task<IActionResult> AnalyzeImage([FromForm] Guid patientProfileId, IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var result = await _gestureCommandService.AnalyzeImageAsync(
                patientProfileId, memoryStream.ToArray(), file.FileName);

            return HandleResult(result);
        }
    }
}