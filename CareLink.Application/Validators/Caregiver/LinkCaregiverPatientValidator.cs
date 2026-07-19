using CareLink.Application.DTOs.Caregiver;
using FluentValidation;

namespace CareLink.Application.Validators.Caregiver
{
    public class LinkCaregiverPatientValidator : AbstractValidator<LinkCaregiverPatientDto>
    {
        public LinkCaregiverPatientValidator()
        {
            RuleFor(x => x.CaregiverProfileId)
                .NotEmpty().WithMessage("CaregiverProfileId is required.");

            RuleFor(x => x.PatientProfileId)
                .NotEmpty().WithMessage("PatientProfileId is required.");
        }
    }
}