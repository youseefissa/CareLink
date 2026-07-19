namespace CareLink.Application.Interfaces
{
    public interface IEmailSender
    {
        Task<bool> SendAsync(string toEmail, string subject, string body);
    }
}