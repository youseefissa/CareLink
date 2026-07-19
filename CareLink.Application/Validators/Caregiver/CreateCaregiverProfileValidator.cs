using CareLink.Application.DTOs.Caregiver;
using FluentValidation;

namespace CareLink.Application.Validators.Caregiver
{
    public class CreateCaregiverProfileValidator : AbstractValidator<CreateCaregiverProfileDto>
    {
        private static readonly string[] AllowedRelationshipTypes =
        {
            "Family", "Nurse", "Professional Caregiver"
        };

        public CreateCaregiverProfileValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.RelationshipType)
                .NotEmpty().WithMessage("Relationship type is required.")
                .Must(value => AllowedRelationshipTypes.Contains(value))
                .WithMessage($"Relationship type must be one of: {string.Join(", ", AllowedRelationshipTypes)}.");
        }
    }
}