using CareLink.Application.DTOs.Gesture;
using FluentValidation;

namespace CareLink.Application.Validators.Gesture
{
    public class GestureCommandValidator : AbstractValidator<GestureCommandDto>
    {
        public GestureCommandValidator()
        {
            RuleFor(x => x.PatientProfileId)
                .NotEmpty().WithMessage("PatientProfileId is required.");

            RuleFor(x => x.Gesture)
                .IsInEnum().WithMessage("Gesture must be a valid value.");

            RuleFor(x => x.Confidence)
                .InclusiveBetween(0, 1).WithMessage("Confidence must be between 0 and 1.");
        }
    }
}