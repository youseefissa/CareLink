using CareLink.Application.DTOs.Auth;
using FluentValidation;

namespace CareLink.Application.Validators.Auth
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDto>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Reset token is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Za-z]").WithMessage("Password must contain at least one letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.");
        }
    }
}