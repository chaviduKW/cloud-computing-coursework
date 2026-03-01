using Microsoft.EntityFrameworkCore;
using VoteApi.Entities;

namespace VoteApi.Data
{
    public class VoteDbContext : DbContext
    {
        public VoteDbContext(DbContextOptions<VoteDbContext> options)
            : base(options) { }

        public DbSet<Vote> Votes => Set<Vote>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("vote");

            modelBuilder.Entity<Vote>(entity =>
            {
                entity.ToTable("votes");

                entity.HasKey(v => v.Id);

                // Store enum as integer
                entity.Property(v => v.VoteType)
                      .HasConversion<int>();

                // Ensure one vote per user per submission
                entity.HasIndex(v => new { v.SalarySubmissionId, v.UserId })
                      .IsUnique();
            });
        }
    }
}
