using System.Collections.ObjectModel;

namespace ApiTestingDashboard.Core.Entities
{
    public class Team : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty; // User ID from Identity

        // Navigation properties
        public virtual ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
        public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();
    }
}