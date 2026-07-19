using CareLink.Application.Common;
using CareLink.Application.DTOs.Notification;

namespace CareLink.Application.Interfaces
{
    public interface INotificationService
    {
        Task<Result> SendAsync(SendNotificationDto request);
        Task<Result<IReadOnlyList<NotificationDto>>> GetForUserAsync(Guid userId);
    }
}