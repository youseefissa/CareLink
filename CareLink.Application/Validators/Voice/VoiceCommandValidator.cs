using CareLink.Application.DTOs.Voice;
using FluentValidation;

namespace CareLink.Application.Validators.Voice
{
    public class VoiceCommandValidator : AbstractValidator<VoiceCommandDto>
    {
        public VoiceCommandValidator()
        {
            RuleFor(x => x.PatientProfileId)
                .NotEmpty().WithMessage("PatientProfileId is required.");

            RuleFor(x => x.RecognizedText)
                .NotEmpty().WithMessage("Recognized text is required.")
                .MaximumLength(500).WithMessage("Recognized text cannot exceed 500 characters.");
        }
    }
}