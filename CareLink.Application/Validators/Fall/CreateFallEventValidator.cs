using CareLink.Application.DTOs.Fall;
using FluentValidation;

namespace CareLink.Application.Validators.Fall
{
    public class CreateFallEventValidator : AbstractValidator<CreateFallEventDto>
    {
        public CreateFallEventValidator()
        {
            RuleFor(x => x.PatientProfileId)
                .NotEmpty().WithMessage("PatientProfileId is required.");

            RuleFor(x => x.Confidence)
                .InclusiveBetween(0, 1).WithMessage("Confidence must be between 0 and 1.");

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