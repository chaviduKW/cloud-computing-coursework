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
            modelBuilder.HasDefaultSchema("community");

            modelBuilder.Entity<Vote>(entity =>
            {
                entity.ToTable("votes");

                entity.HasKey(v => v.Id);

                entity.Property(v => v.Id).HasColumnName("id");
                entity.Property(v => v.SalarySubmissionId).HasColumnName("salarysubmissionid");
                entity.Property(v => v.UserId).HasColumnName("userid");
                entity.Property(v => v.VoteType).HasColumnName("votetype").HasConversion<int>();
                entity.Property(v => v.CreatedAt).HasColumnName("createdat");

                // Ensure one vote per user per submission
                entity.HasIndex(v => new { v.SalarySubmissionId, v.UserId })
                      .IsUnique();
            });
        }
    }
}
