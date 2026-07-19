using CareLink.Application.DTOs.Notification;
using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [Authorize(Roles = "Patient,Caregiver,Admin")]
    public class NotificationController : BaseApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendNotificationDto request)
        {
            var result = await _notificationService.SendAsync(request);
            return HandleResult(result);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetForUser(Guid userId)
        {
            var result = await _notificationService.GetForUserAsync(userId);
            return HandleResult(result);
        }
    }
}