using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ApiTestingDashboard.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ApiTestingDashboard.Infrastructure.Data
{
    // Inherites from IdentiyDbContext to get user management tables
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Our domain tables
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Endpoint> Endpoints { get; set; }
        public DbSet<RequestTemplate> RequestTemplates { get; set; }
        public DbSet<ResponseHistory> ResponseHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call base to configure Identity tables
            base.OnModelCreating(builder);

            // Configure our custom entities
            ConfigureTeams(builder);
            ConfigureCollections(builder);
            ConfigureEndpoints(builder);
            ConfigureRequestTemplates(builder);
            ConfigureResponseHistory(builder);
        }

        private void ConfigureTeams(ModelBuilder builder)
        {
            builder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.OwnerId).IsRequired().HasMaxLength(450); // Identity User Length

                // Index for faster queries
                entity.HasIndex(e => e.OwnerId);
            });

            builder.Entity<TeamMember>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);

                // Relationship: Team has many TeamMembers
                entity.HasOne(e => e.Team)
                    .WithMany(t => t.Members)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint: user can only be in team once
                entity.HasIndex(e => new { e.UserId, e.TeamId }).IsUnique();
            });
        }

        public void ConfigureCollections(ModelBuilder builder)
        {
            builder.Entity<Collection>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.OwnerId).IsRequired().HasMaxLength(450);

                // Relationship: Team has many Collections (optional)
                entity.HasOne(e => e.Team)
                    .WithMany(t => t.Collections)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.SetNull); // If team deleted, collection becomes personal

                entity.HasIndex(e => e.OwnerId);
                entity.HasIndex(e => e.TeamId);
            });
        }

        private void ConfigureEndpoints(ModelBuilder builder)
        {
            builder.Entity<Endpoint>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Url).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.FolderPath).HasMaxLength(500);

                // Enum stored as integer
                entity.Property(e => e.Method).HasConversion<int>();

                // Relationship: Collection has many Endpoints
                entity.HasOne(e => e.Collection)
                    .WithMany(c => c.Endpoints)
                    .HasForeignKey(e => e.CollectionId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index for faster collection queries
                entity.HasIndex(e => e.CollectionId);
            });
        }

        private void ConfigureRequestTemplates(ModelBuilder builder)
        {
            builder.Entity<RequestTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Headers).HasColumnType("text");
                entity.Property(e => e.Body).HasColumnType("text");
                entity.Property(e => e.AuthConfig).HasColumnType("text");

                // Enum stored as integer
                entity.Property(e => e.AuthType).HasConversion<int>();

                // Relationship: Endpoint has many RequestTemplates
                entity.HasOne(e => e.Endpoint)
                    .WithMany(ep => ep.Templates)
                    .HasForeignKey(e => e.EndpointId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.EndpointId);
            });
        }

        private void ConfigureResponseHistory(ModelBuilder builder)
        {
            builder.Entity<ResponseHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Headers).HasColumnType("text");
                entity.Property(e => e.Body).HasColumnType("text");
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);

                // Relationship: Endpoint has many ResponseHistory
                entity.HasOne(e => e.Endpoint)
                    .WithMany(ep => ep.ResponseHistory)
                    .HasForeignKey(e => e.EndpointId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes for performance queries
                entity.HasIndex(e => e.EndpointId);
                entity.HasIndex(e => e.ExecutedAt);
                entity.HasIndex(e => e.UserId);
            });
        }
    }
}