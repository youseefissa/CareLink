using CareLink.Domain.Interfaces.Repositories;

namespace CareLink.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CareLinkDbContext _context;

        private IUserRepository? _users;
        private IPatientProfileRepository? _patientProfiles;
        private ICaregiverProfileRepository? _caregiverProfiles;
        private ICaregiverPatientLinkRepository? _caregiverPatientLinks;
        private IFallEventRepository? _fallEvents;
        private ISOSEventRepository? _sosEvents;
        private IActivityLogRepository? _activityLogs;
        private IAlertRepository? _alerts;
        private IVoiceCommandLogRepository? _voiceCommandLogs;
        private IGestureCommandLogRepository? _gestureCommandLogs;
        private INotificationLogRepository? _notificationLogs;
        private ISafetyRecommendationRepository? _safetyRecommendations;
        private ITrendReportRepository? _trendReports;
        private IRefreshTokenRepository? _refreshTokens;
        private IPasswordResetTokenRepository? _passwordResetTokens;

        public UnitOfWork(CareLinkDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IPatientProfileRepository PatientProfiles => _patientProfiles ??= new PatientProfileRepository(_context);
        public ICaregiverProfileRepository CaregiverProfiles => _caregiverProfiles ??= new CaregiverProfileRepository(_context);
        public ICaregiverPatientLinkRepository CaregiverPatientLinks => _caregiverPatientLinks ??= new CaregiverPatientLinkRepository(_context);
        public IFallEventRepository FallEvents => _fallEvents ??= new FallEventRepository(_context);
        public ISOSEventRepository SOSEvents => _sosEvents ??= new SOSEventRepository(_context);
        public IActivityLogRepository ActivityLogs => _activityLogs ??= new ActivityLogRepository(_context);
        public IAlertRepository Alerts => _alerts ??= new AlertRepository(_context);
        public IVoiceCommandLogRepository VoiceCommandLogs => _voiceCommandLogs ??= new VoiceCommandLogRepository(_context);
        public IGestureCommandLogRepository GestureCommandLogs => _gestureCommandLogs ??= new GestureCommandLogRepository(_context);
        public INotificationLogRepository NotificationLogs => _notificationLogs ??= new NotificationLogRepository(_context);
        public ISafetyRecommendationRepository SafetyRecommendations => _safetyRecommendations ??= new SafetyRecommendationRepository(_context);
        public ITrendReportRepository TrendReports => _trendReports ??= new TrendReportRepository(_context);
        public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);
        public IPasswordResetTokenRepository PasswordResetTokens => _passwordResetTokens ??= new PasswordResetTokenRepository(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}