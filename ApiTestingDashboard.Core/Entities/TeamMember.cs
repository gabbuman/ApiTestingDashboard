namespace ApiTestingDashboard.Core.Entities
{
    public enum TeamRole
    {
        Owner = 0,
        Editor = 1,
        Viewer = 2
    }

    public class TeamMember : BaseEntity
    {
        public string UserId { get; set; } = string.Empty; // User ID from Identity
        public int TeamId { get; set; }
        public TeamRole Role { get; set; }

        // Navigation properties
        public virtual Team Team { get; set; } = null!;
    }
}