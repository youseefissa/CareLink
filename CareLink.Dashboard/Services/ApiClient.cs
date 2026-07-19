using CareLink.Dashboard.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CareLink.Dashboard.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;

        public ApiClient(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        private async Task AttachTokenAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var token = authState.User.FindFirst("access_token")?.Value;

            _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<(bool Success, T? Data, List<string> Errors)> GetAsync<T>(string endpoint)
        {
            await AttachTokenAsync();

            var response = await _httpClient.GetAsync(endpoint);
            return await ParseResponseAsync<T>(response);
        }

        public async Task<(bool Success, T? Data, List<string> Errors)> PostAsync<T>(string endpoint, object body)
        {
            await AttachTokenAsync();

            var response = await _httpClient.PostAsJsonAsync(endpoint, body);
            return await ParseResponseAsync<T>(response);
        }

        public async Task<(bool Success, List<string> Errors)> PostAsync(string endpoint, object body)
        {
            await AttachTokenAsync();

            var response = await _httpClient.PostAsJsonAsync(endpoint, body);

            if (response.IsSuccessStatusCode)
                return (true, new List<string>());

            var errors = await ExtractErrorsAsync(response);
            return (false, errors);
        }

        private async Task<(bool Success, T? Data, List<string> Errors)> ParseResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<T>();
                return (true, data, new List<string>());
            }

            var errors = await ExtractErrorsAsync(response);
            return (false, default, errors);
        }

        private static async Task<List<string>> ExtractErrorsAsync(HttpResponseMessage response)
        {
            try
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return errorResponse?.Errors ?? new List<string> { "An unexpected error occurred." };
            }
            catch
            {
                return new List<string> { "An unexpected error occurred." };
            }
        }
    }
}