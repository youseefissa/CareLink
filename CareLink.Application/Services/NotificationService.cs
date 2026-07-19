using CareLink.Application.Common;
using CareLink.Application.DTOs.Notification;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Enums;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPushNotificationSender _pushNotificationSender;
        private readonly ICurrentUserService _currentUser;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IPushNotificationSender pushNotificationSender,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _pushNotificationSender = pushNotificationSender;
            _currentUser = currentUser;
        }

        public async Task<Result> SendAsync(SendNotificationDto request)
        {
            if (_currentUser.Role != "Admin")
                return Result.Failure("Only an admin can manually trigger a notification.");

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user is null)
                return Result.Failure("User not found.");

            var log = new NotificationLog
            {
                UserId = request.UserId,
                Title = request.Title,
                Body = request.Body,
                Status = NotificationStatus.Pending
            };

            await _unitOfWork.NotificationLogs.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();

            if (string.IsNullOrWhiteSpace(user.FcmDeviceToken))
            {
                log.Status = NotificationStatus.Failed;
                _unitOfWork.NotificationLogs.Update(log);
                await _unitOfWork.SaveChangesAsync();

                return Result.Failure("User has no registered device token.");
            }

            var wasSent = await _pushNotificationSender.SendAsync(user.FcmDeviceToken, request.Title, request.Body);

            log.Status = wasSent ? NotificationStatus.Sent : NotificationStatus.Failed;
            log.DeliveredAt = wasSent ? DateTime.UtcNow : null;

            _unitOfWork.NotificationLogs.Update(log);
            await _unitOfWork.SaveChangesAsync();

            return wasSent ? Result.Success() : Result.Failure("Failed to deliver push notification.");
        }

        public async Task<Result<IReadOnlyList<NotificationDto>>> GetForUserAsync(Guid userId)
        {
            var isOwner = _currentUser.UserId == userId;
            var isAdmin = _currentUser.Role == "Admin";

            if (!isOwner && !isAdmin)
                return Result<IReadOnlyList<NotificationDto>>.Failure("You do not have permission to view these notifications.");

            var logs = await _unitOfWork.NotificationLogs.GetByUserIdAsync(userId);

            var dtoList = logs.Select(log => new NotificationDto
            {
                Id = log.Id,
                Title = log.Title,
                Body = log.Body,
                Status = log.Status,
                DeliveredAt = log.DeliveredAt
            }).ToList();

            return Result<IReadOnlyList<NotificationDto>>.Success(dtoList);
        }
    }
}