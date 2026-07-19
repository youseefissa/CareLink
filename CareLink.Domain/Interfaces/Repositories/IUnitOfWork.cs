namespace CareLink.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IPatientProfileRepository PatientProfiles { get; }
        ICaregiverProfileRepository CaregiverProfiles { get; }
        ICaregiverPatientLinkRepository CaregiverPatientLinks { get; }
        IFallEventRepository FallEvents { get; }
        ISOSEventRepository SOSEvents { get; }
        IActivityLogRepository ActivityLogs { get; }
        IAlertRepository Alerts { get; }
        IVoiceCommandLogRepository VoiceCommandLogs { get; }
        IGestureCommandLogRepository GestureCommandLogs { get; }
        INotificationLogRepository NotificationLogs { get; }
        ISafetyRecommendationRepository SafetyRecommendations { get; }
        ITrendReportRepository TrendReports { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IPasswordResetTokenRepository PasswordResetTokens { get; }

        Task<int> SaveChangesAsync();
    }
}