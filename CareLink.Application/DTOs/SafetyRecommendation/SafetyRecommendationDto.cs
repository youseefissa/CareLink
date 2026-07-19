namespace CareLink.Application.DTOs.SafetyRecommendation
{
    public class SafetyRecommendationDto
    {
        public Guid Id { get; set; }
        public string RecommendationText { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsAcknowledged { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}