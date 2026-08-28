using CareLink.Application.DTOs.Alert;
using CareLink.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CareLink.Infrastructure.SignalR
{
    public class SignalRAlertNotifier : IAlertNotifier
    {
        private readonly IHubContext<AlertsHub> _hubContext;

        public SignalRAlertNotifier(IHubContext<AlertsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewAlertAsync(AlertBroadcastDto alert)
        {
            await _hubContext.Clients
                .Group(alert.PatientProfileId.ToString())
                .SendAsync("ReceiveAlert", alert);
        }
    }
}