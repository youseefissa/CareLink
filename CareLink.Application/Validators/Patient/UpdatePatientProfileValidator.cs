using CareLink.Application.DTOs.Patient;
using FluentValidation;

namespace CareLink.Application.Validators.Patient
{
    public class UpdatePatientProfileValidator : AbstractValidator<UpdatePatientProfileDto>
    {
        public UpdatePatientProfileValidator()
        {
            RuleFor(x => x.MedicalNotes)
                .MaximumLength(2000).WithMessage("Medical notes cannot exceed 2000 characters.");

            RuleFor(x => x.EmergencyContactPhone)
                .Matches(@"^\+?[0-9]{10,15}$")
                .When(x => !string.IsNullOrWhiteSpace(x.EmergencyContactPhone))
                .WithMessage("Emergency contact phone format is invalid.");

            RuleFor(x => x.SleepWindowStart)
                .InclusiveBetween(TimeSpan.Zero, new TimeSpan(23, 59, 59))
                .When(x => x.SleepWindowStart.HasValue)
                .WithMessage("Sleep window start must be a valid time of day.");

            RuleFor(x => x.SleepWindowEnd)
                .InclusiveBetween(TimeSpan.Zero, new TimeSpan(23, 59, 59))
                .When(x => x.SleepWindowEnd.HasValue)
                .WithMessage("Sleep window end must be a valid time of day.");
        }
    }
}