using CareLink.Domain.Enums;

namespace CareLink.Application.DTOs.Alert
{
    public class AlertDto
    {
        public Guid Id { get; set; }
        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}