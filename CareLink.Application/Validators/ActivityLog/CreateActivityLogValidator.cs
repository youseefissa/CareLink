using CareLink.Application.DTOs.ActivityLog;
using FluentValidation;

namespace CareLink.Application.Validators.ActivityLog
{
    public class CreateActivityLogValidator : AbstractValidator<CreateActivityLogDto>
    {
        private static readonly string[] AllowedActivityTypes =
        {
            "Movement", "Inactivity", "DeviceCheckIn"
        };

        public CreateActivityLogValidator()
        {
            RuleFor(x => x.PatientProfileId)
                .NotEmpty().WithMessage("PatientProfileId is required.");

            RuleFor(x => x.ActivityType)
                .NotEmpty().WithMessage("Activity type is required.")
                .Must(value => AllowedActivityTypes.Contains(value))
                .WithMessage($"Activity type must be one of: {string.Join(", ", AllowedActivityTypes)}.");

            RuleFor(x => x.Details)
                .MaximumLength(500).WithMessage("Details cannot exceed 500 characters.");
        }
    }
}