using CareLink.Application.Common;
using CareLink.Application.DTOs.Caregiver;
using CareLink.Application.Interfaces;
using CareLink.Domain.Entities;
using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Application.Services
{
    public class CaregiverDashboardService : ICaregiverDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CaregiverDashboardService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> LinkPatientAsync(LinkCaregiverPatientDto request)
        {
            var caregiver = await _unitOfWork.CaregiverProfiles.GetByIdAsync(request.CaregiverProfileId);
            if (caregiver is null)
                return Result.Failure("Caregiver profile not found.");

            var isOwner = _currentUser.UserId == caregiver.UserId;
            var isAdmin = _currentUser.Role == "Admin";

            if (!isOwner && !isAdmin)
                return Result.Failure("You do not have permission to modify this caregiver's links.");

            var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(request.PatientProfileId);
            if (patient is null)
                return Result.Failure("Patient profile not found.");

            var alreadyLinked = await _unitOfWork.CaregiverPatientLinks.LinkExistsAsync(
                request.CaregiverProfileId, request.PatientProfileId);

            if (alreadyLinked)
                return Result.Failure("This caregiver is already linked to this patient.");

            var link = new CaregiverPatientLink
            {
                CaregiverProfileId = request.CaregiverProfileId,
                PatientProfileId = request.PatientProfileId,
                IsPrimaryCaregiver = request.IsPrimaryCaregiver
            };

            await _unitOfWork.CaregiverPatientLinks.AddAsync(link);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<IReadOnlyList<CaregiverDashboardDto>>> GetDashboardAsync(Guid caregiverProfileId)
        {
            var caregiver = await _unitOfWork.CaregiverProfiles.GetByIdAsync(caregiverProfileId);
            if (caregiver is null)
                return Result<IReadOnlyList<CaregiverDashboardDto>>.Failure("Caregiver profile not found.");

            var isOwner = _currentUser.UserId == caregiver.UserId;
            var isAdmin = _currentUser.Role == "Admin";

            if (!isOwner && !isAdmin)
                return Result<IReadOnlyList<CaregiverDashboardDto>>.Failure("You do not have permission to view this dashboard.");

            var patients = await _unitOfWork.PatientProfiles.GetByCaregiverIdAsync(caregiverProfileId);

            var dashboardList = new List<CaregiverDashboardDto>();

            foreach (var patient in patients)
            {
                var lastActivity = await _unitOfWork.ActivityLogs.GetLastActivityTimeAsync(patient.Id);
                var unresolvedAlerts = await _unitOfWork.Alerts.GetUnresolvedAsync(patient.Id);
                var weekStart = DateTime.UtcNow.AddDays(-7);
                var fallsThisWeek = await _unitOfWork.FallEvents.CountFallsInPeriodAsync(patient.Id, weekStart, DateTime.UtcNow);
                var recentFalls = await _unitOfWork.FallEvents.GetRecentFallsAsync(patient.Id, 1);

                dashboardList.Add(new CaregiverDashboardDto
                {
                    PatientProfileId = patient.Id,
                    PatientFullName = patient.User?.FullName ?? string.Empty,
                    LastActivityAt = lastActivity,
                    LastFallAt = recentFalls.FirstOrDefault()?.CreatedAt,
                    UnresolvedAlertsCount = unresolvedAlerts.Count,
                    FallsThisWeek = fallsThisWeek
                });
            }

            return Result<IReadOnlyList<CaregiverDashboardDto>>.Success(dashboardList);
        }
    }
}