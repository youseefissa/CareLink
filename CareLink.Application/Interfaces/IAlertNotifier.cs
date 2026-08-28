using CareLink.Application.DTOs.Alert;

namespace CareLink.Application.Interfaces
{
    public interface IAlertNotifier
    {
        Task NotifyNewAlertAsync(AlertBroadcastDto alert);
    }
}