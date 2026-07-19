using CareLink.Application.DTOs.SOS;
using FluentValidation;

namespace CareLink.Application.Validators.SOS
{
    public class CreateSOSEventValidator : AbstractValidator<CreateSOSEventDto>
    {
        private static readonly string[] AllowedTriggerSources =
        {
            "Button", "Voice", "Gesture"
        };

        public CreateSOSEventValidator()
        {
            RuleFor(x => x.PatientProfileId)
                .NotEmpty().WithMessage("PatientProfileId is required.");

            RuleFor(x => x.TriggerSource)
                .NotEmpty().WithMessage("Trigger source is required.")
                .Must(value => AllowedTriggerSources.Contains(value))
                .WithMessage($"Trigger source must be one of: {string.Join(", ", AllowedTriggerSources)}.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .When(x => x.Latitude.HasValue)
                .WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .When(x => x.Longitude.HasValue)
                .WithMessage("Longitude must be between -180 and 180.");
        }
    }
}