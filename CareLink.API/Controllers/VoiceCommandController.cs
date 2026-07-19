using CareLink.Application.DTOs.Voice;
using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [Authorize(Roles = "Patient,Caregiver,Admin")]
    public class VoiceCommandController : BaseApiController
    {
        private readonly IVoiceCommandService _voiceCommandService;

        public VoiceCommandController(IVoiceCommandService voiceCommandService)
        {
            _voiceCommandService = voiceCommandService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] VoiceCommandDto request)
        {
            var result = await _voiceCommandService.ProcessCommandAsync(request);
            return HandleResult(result);
        }
    }
}