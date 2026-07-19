using CareLink.Application.Common;
using CareLink.Application.DTOs.TrendReport;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class TrendReportService : ITrendReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public TrendReportService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<TrendReportDto>> GenerateAsync(GenerateTrendReportDto request)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(request.PatientProfileId);
            if (patient is null)
                return Result<TrendReportDto>.Failure("Patient profile not found.");

            var canAccess = await CanAccessPatientAsync(patient);
            if (!canAccess)
                return Result<TrendReportDto>.Failure("You do not have permission to generate a report for this patient.");

            var totalFalls = await _unitOfWork.FallEvents.CountFallsInPeriodAsync(
                request.PatientProfileId, request.PeriodStart, request.PeriodEnd);

            var averageDailyActivity = await _unitOfWork.ActivityLogs.GetAverageDailyActivityAsync(
                request.PatientProfileId, request.PeriodStart, request.PeriodEnd);

            var inactivityEventsCount = await CountInactivityAlertsAsync(
                request.PatientProfileId, request.PeriodStart, request.PeriodEnd);

            var report = new TrendReport
            {
                PatientProfileId = request.PatientProfileId,
                PeriodStart = request.PeriodStart,
                PeriodEnd = request.PeriodEnd,
                TotalFalls = totalFalls,
                AverageDailyActivity = averageDailyActivity,
                MedicationConfirmationsCount = 0,
                MedicationMissedCount = 0,
                InactivityEventsCount = inactivityEventsCount,
                GeneratedPdfPath = null
            };

            await _unitOfWork.TrendReports.AddAsync(report);
            await _unitOfWork.SaveChangesAsync();

            return Result<TrendReportDto>.Success(MapToDto(report));
        }

        public async Task<Result<IReadOnlyList<TrendReportDto>>> GetHistoryAsync(Guid patientProfileId)
        {
            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(patientProfileId);
            if (patient is null)
                return Result<IReadOnlyList<TrendReportDto>>.Failure("Patient profile not found.");

            var canAccess = await CanAccessPatientAsync(patient);
            if (!canAccess)
                return Result<IReadOnlyList<TrendReportDto>>.Failure("You do not have permission to view these reports.");

            var reports = await _unitOfWork.TrendReports.GetByPatientIdAsync(patientProfileId);

            var dtoList = reports.Select(MapToDto).ToList();

            return Result<IReadOnlyList<TrendReportDto>>.Success(dtoList);
        }

        private async Task<bool> CanAccessPatientAsync(PatientProfile profile)
        {
            if (_currentUser.Role == "Admin")
                return true;

            if (_currentUser.Role == "Caregiver" && _currentUser.UserId.HasValue)
            {
                var caregiverProfile = await _unitOfWork.CaregiverProfiles.GetByUserIdAsync(_currentUser.UserId.Value);
                if (caregiverProfile is null)
                    return false;

                return await _unitOfWork.CaregiverPatientLinks.LinkExistsAsync(caregiverProfile.Id, profile.Id);
            }

            return false;
        }

        private async Task<int> CountInactivityAlertsAsync(Guid patientProfileId, DateTime start, DateTime end)
        {
            var alerts = await _unitOfWork.Alerts.GetByPatientIdAsync(patientProfileId);

            return alerts.Count(a =>
                a.Type == Domain.Enums.AlertType.NoMovement &&
                a.CreatedAt >= start &&
                a.CreatedAt <= end);
        }

        private static TrendReportDto MapToDto(TrendReport report) => new()
        {
            Id = report.Id,
            PeriodStart = report.PeriodStart,
            PeriodEnd = report.PeriodEnd,
            TotalFalls = report.TotalFalls,
            AverageDailyActivity = report.AverageDailyActivity,
            MedicationConfirmationsCount = report.MedicationConfirmationsCount,
            MedicationMissedCount = report.MedicationMissedCount,
            InactivityEventsCount = report.InactivityEventsCount,
            GeneratedPdfPath = report.GeneratedPdfPath
        };
    }
}