using CareLink.Application.DTOs.Notification;
using FluentValidation;

namespace CareLink.Application.Validators.Notification
{
    public class SendNotificationValidator : AbstractValidator<SendNotificationDto>
    {
        public SendNotificationValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Body is required.")
                .MaximumLength(1000).WithMessage("Body cannot exceed 1000 characters.");
        }
    }
}