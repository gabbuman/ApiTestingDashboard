namespace ApiTestingDashboard.Core.Entities
{
    public class ResponseHistory : BaseEntity
    {
        public int StatusCode { get; set; }
        public string Headers { get; set; } = "{}"; // JSON string
        public string Body { get; set; } = string.Empty;
        public int ResponseTimeMs { get; set; }
        public long ResponseSizeBytes { get; set; }
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
        public int EndpointId { get; set; }
        public string UserId { get; set; } = string.Empty; // Who executed the request

        // Navigation properties
        public virtual Endpoint Endpoint { get; set; } = null!;
    }
}