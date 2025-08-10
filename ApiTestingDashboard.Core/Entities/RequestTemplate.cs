namespace ApiTestingDashboard.Core.Entities
{
    public enum AuthType
    {
        None = 0,
        BearerToken = 1,
        BasicAuth = 2,
        ApiKey = 3
    }

    public class RequestTemplate : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Headers { get; set; } = "{}"; // JSON String
        public string Body { get; set; } = string.Empty;
        public AuthType AuthType { get; set; }
        public string AuthConfig { get; set; } = "{}"; // JSON string for auth details
        public int EndpointId { get; set; }

        // Navigation Properties
        public virtual Endpoint Endpoint { get; set; } = null!;
    }
}