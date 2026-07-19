using CareLink.Application.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CareLink.Infrastructure.ExternalServices
{
    public class FirebaseNotificationSender : IPushNotificationSender
    {
        private readonly ILogger<FirebaseNotificationSender> _logger;
        private static bool _isInitialized;
        private static readonly object InitLock = new();

        public FirebaseNotificationSender(IConfiguration configuration, ILogger<FirebaseNotificationSender> logger)
        {
            _logger = logger;
            EnsureInitialized(configuration);
        }

        private void EnsureInitialized(IConfiguration configuration)
        {
            if (_isInitialized)
                return;

            lock (InitLock)
            {
                if (_isInitialized)
                    return;

                var credentialsPath = configuration["Firebase:CredentialsPath"];

                if (string.IsNullOrWhiteSpace(credentialsPath) || !File.Exists(credentialsPath))
                {
                    _logger.LogWarning("Firebase credentials file not found. Push notifications will be disabled until configured.");
                    return;
                }

                if (FirebaseApp.DefaultInstance is null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(credentialsPath)
                    });
                }

                _isInitialized = true;
            }
        }

        public async Task<bool> SendAsync(string deviceToken, string title, string body)
        {
            if (FirebaseApp.DefaultInstance is null)
            {
                _logger.LogWarning("Firebase is not configured. Notification was not sent, only logged.");
                return false;
            }

            try
            {
                var message = new Message
                {
                    Token = deviceToken,
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = title,
                        Body = body
                    }
                };

                await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send Firebase push notification.");
                return false;
            }
        }
    }
}