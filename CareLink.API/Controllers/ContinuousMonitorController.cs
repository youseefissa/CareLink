using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [Authorize(Roles = "Patient,Admin")]
    public class ContinuousMonitorController : BaseApiController
    {
        private readonly IContinuousMonitorService _continuousMonitorService;

        public ContinuousMonitorController(IContinuousMonitorService continuousMonitorService)
        {
            _continuousMonitorService = continuousMonitorService;
        }

        [HttpPost("frame")]
        public async Task<IActionResult> ProcessFrame([FromForm] Guid patientProfileId, IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var result = await _continuousMonitorService.ProcessFrameAsync(
                patientProfileId, memoryStream.ToArray(), file.FileName);

            return HandleResult(result);
        }
    }
}