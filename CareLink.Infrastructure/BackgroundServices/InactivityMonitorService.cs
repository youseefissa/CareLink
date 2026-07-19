using CareLink.Domain.Entities;
using CareLink.Domain.Enums;
using CareLink.Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CareLink.Infrastructure.BackgroundServices
{
    public class InactivityMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InactivityMonitorService> _logger;

        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan NormalInactivityThreshold = TimeSpan.FromHours(2);
        private static readonly TimeSpan MaxSleepInactivityThreshold = TimeSpan.FromHours(10);

        public InactivityMonitorService(IServiceProvider serviceProvider, ILogger<InactivityMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAllPatientsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking patient inactivity.");
                }

                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        private async Task CheckAllPatientsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var patients = await unitOfWork.PatientProfiles.GetAllAsync();
            var utcNow = DateTime.UtcNow;

            foreach (var patient in patients)
            {
                var lastActivity = await unitOfWork.ActivityLogs.GetLastActivityTimeAsync(patient.Id);

                var inactiveDuration = lastActivity is null
                    ? TimeSpan.MaxValue
                    : utcNow - lastActivity.Value;

                var isCurrentlyInSleepWindow = IsWithinSleepWindow(
                    utcNow.TimeOfDay, patient.SleepWindowStart, patient.SleepWindowEnd);

                var effectiveThreshold = isCurrentlyInSleepWindow
                    ? MaxSleepInactivityThreshold
                    : NormalInactivityThreshold;

                var isInactiveTooLong = inactiveDuration >= effectiveThreshold;

                if (!isInactiveTooLong)
                    continue;

                var existingUnresolvedAlerts = await unitOfWork.Alerts.GetUnresolvedAsync(patient.Id);
                var alreadyHasNoMovementAlert = existingUnresolvedAlerts.Any(a => a.Type == AlertType.NoMovement);

                if (alreadyHasNoMovementAlert)
                    continue;

                var message = isCurrentlyInSleepWindow
                    ? "No movement detected for an unusually long time, even accounting for sleep hours."
                    : "No movement detected for two hours.";

                var alert = new Alert
                {
                    PatientProfileId = patient.Id,
                    Type = AlertType.NoMovement,
                    Severity = AlertSeverity.High,
                    Message = message,
                    IsResolved = false
                };

                await unitOfWork.Alerts.AddAsync(alert);
            }

            await unitOfWork.SaveChangesAsync();
        }

        private static bool IsWithinSleepWindow(TimeSpan currentTime, TimeSpan sleepStart, TimeSpan sleepEnd)
        {
            if (sleepStart == sleepEnd)
                return false;

            if (sleepStart < sleepEnd)
            {
                return currentTime >= sleepStart && currentTime <= sleepEnd;
            }

            return currentTime >= sleepStart || currentTime <= sleepEnd;
        }
    }
}