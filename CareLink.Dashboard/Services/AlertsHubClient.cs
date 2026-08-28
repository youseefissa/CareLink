using CareLink.Dashboard.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace CareLink.Dashboard.Services
{
    public class AlertsHubClient : IAsyncDisposable
    {
        private HubConnection? _connection;

        public event Action<AlertItem>? OnAlertReceived;

        public async Task ConnectAsync(string apiBaseUrl, string accessToken)
        {
            if (_connection is not null)
                return;

            _connection = new HubConnectionBuilder()
                .WithUrl($"{apiBaseUrl.TrimEnd('/')}/hubs/alerts", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(accessToken)!;
                })
                .WithAutomaticReconnect()
                .Build();

            _connection.On<AlertItem>("ReceiveAlert", alert =>
            {
                OnAlertReceived?.Invoke(alert);
            });

            await _connection.StartAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                await _connection.DisposeAsync();
            }
        }
    }
}