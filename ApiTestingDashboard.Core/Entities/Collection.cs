namespace ApiTestingDashboard.Core.Entities
{
    public class Collection : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty; // User ID from Identity
        public int? TeamId { get; set; } // Nullable - can be personal or team collection

        // Navigation properties
        public virtual Team? Team { get; set; }
        public virtual ICollection<Endpoint> Endpoints { get; set; } = new List<Endpoint>();
    }
}