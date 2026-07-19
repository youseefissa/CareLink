namespace CareLink.Application.Interfaces
{
    public interface IPushNotificationSender
    {
        Task<bool> SendAsync(string deviceToken, string title, string body);
    }
}