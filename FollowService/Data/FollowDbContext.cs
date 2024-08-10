using Microsoft.EntityFrameworkCore;
using FollowService.Models;

namespace FollowService.Data
{
    public class FollowDbContext : DbContext
    {
        public FollowDbContext(DbContextOptions<FollowDbContext> options) : base(options) { }

        public DbSet<Follow> Follows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Follow>()
                .HasKey(f => f.FollowId);

            modelBuilder.Entity<Follow>()
                .Property(f => f.FollowId)
                .ValueGeneratedOnAdd(); // Make FollowId an identity column

            modelBuilder.Entity<Follow>()
                .Property(f => f.FollowedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}