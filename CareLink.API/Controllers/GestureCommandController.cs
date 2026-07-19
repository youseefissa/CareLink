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
    }
}