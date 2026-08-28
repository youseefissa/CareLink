using CareLink.Application.Interfaces;
using CareLink.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CareLink.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<ICaregiverService, CaregiverService>();
            services.AddScoped<ICaregiverDashboardService, CaregiverDashboardService>();
            services.AddScoped<ISOSService, SOSService>();
            services.AddScoped<IFallDetectionService, FallDetectionService>();
            services.AddScoped<IActivityLogService, ActivityLogService>();
            services.AddScoped<IVoiceCommandService, VoiceCommandService>();
            services.AddScoped<IGestureCommandService, GestureCommandService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISafetyRecommendationService, SafetyRecommendationService>();
            services.AddScoped<ITrendReportService, TrendReportService>();
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IContinuousMonitorService, ContinuousMonitorService>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}