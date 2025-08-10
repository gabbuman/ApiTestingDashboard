namespace ApiTestingDashboard.Core.Entities
{
    public enum HttpMethod
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        DELETE = 3,
        PATCH = 4,
        HEAD = 5,
        OPTIONS = 6
    }

    public class Endpoint : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public HttpMethod Method { get; set; }
        public string Description { get; set; } = string.Empty;
        public string FolderPath { get; set; } = string.Empty; // For organization
        public int CollectionId { get; set; }

        // Navigation properties
        public virtual Collection Collection { get; set; } = null!;
        public virtual ICollection<RequestTemplate> Templates { get; set; } = new List<RequestTemplate>();
        public virtual ICollection<ResponseHistory> ResponseHistory { get; set; } = new List<ResponseHistory>();
    }
}