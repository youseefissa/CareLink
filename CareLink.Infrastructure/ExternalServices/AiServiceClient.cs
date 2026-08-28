using CareLink.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CareLink.Infrastructure.ExternalServices
{
    public class AiServiceClient : IAiServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AiServiceClient> _logger;

        public AiServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<AiServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var baseUrl = configuration["AiService:BaseUrl"] ?? "http://127.0.0.1:8000";
            _httpClient.BaseAddress = new Uri(baseUrl);
        }
public async Task<AiGestureResult> AnalyzeHandGestureAsync(byte[] imageBytes, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                content.Add(imageContent, "file", string.IsNullOrWhiteSpace(fileName) ? "upload.jpg" : fileName);

                var response = await _httpClient.PostAsync("/analyze/hand-gesture", content);

                var json = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("AI Service raw response: {StatusCode} - {Body}", response.StatusCode, json);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("AI Service returned status code {StatusCode}", response.StatusCode);
                    return new AiGestureResult { Detected = false };
                }

                var result = JsonSerializer.Deserialize<AiGestureResult>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new AiGestureResult { Detected = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reach AI Service for hand gesture analysis.");
                return new AiGestureResult { Detected = false };
            }
        }
        public async Task<AiFallResult> AnalyzeFallAsync(byte[] imageBytes, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                content.Add(imageContent, "file", string.IsNullOrWhiteSpace(fileName) ? "upload.jpg" : fileName);

                var response = await _httpClient.PostAsync("/analyze/fall-detection", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("AI Service returned status code {StatusCode}", response.StatusCode);
                    return new AiFallResult { Detected = false };
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AiFallResult>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new AiFallResult { Detected = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reach AI Service for fall analysis.");
                return new AiFallResult { Detected = false };
            }
        }
        public async Task<AiContinuousMonitorResult> AnalyzeContinuousFrameAsync(Guid patientProfileId, byte[] imageBytes, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                content.Add(imageContent, "file", string.IsNullOrWhiteSpace(fileName) ? "frame.jpg" : fileName);

                var response = await _httpClient.PostAsync(
                    $"/analyze/continuous-monitor?patient_id={patientProfileId}", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("AI Service returned status code {StatusCode}", response.StatusCode);
                    return new AiContinuousMonitorResult { Detected = false };
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AiContinuousMonitorResult>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new AiContinuousMonitorResult { Detected = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reach AI Service for continuous monitoring.");
                return new AiContinuousMonitorResult { Detected = false };
            }
        }
    }

}