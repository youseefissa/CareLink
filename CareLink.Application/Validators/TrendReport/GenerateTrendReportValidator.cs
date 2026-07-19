using CareLink.Application.DTOs.TrendReport;
using FluentValidation;

namespace CareLink.Application.Validators.TrendReport
{
    public class GenerateTrendReportValidator : AbstractValidator<GenerateTrendReportDto>
    {
        public GenerateTrendReportValidator()
        {
            RuleFor(x => x.PatientProfileId)
                .NotEmpty().WithMessage("PatientProfileId is required.");

            RuleFor(x => x.PeriodStart)
                .NotEmpty().WithMessage("Period start is required.");

            RuleFor(x => x.PeriodEnd)
                .NotEmpty().WithMessage("Period end is required.")
                .GreaterThan(x => x.PeriodStart).WithMessage("Period end must be after period start.");

            RuleFor(x => x.PeriodEnd)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Period end cannot be in the future.");
        }
    }
}