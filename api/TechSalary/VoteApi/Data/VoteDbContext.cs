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
            modelBuilder.Entity<Vote>(entity =>
            {
                entity.ToTable("Votes"); // table name

                entity.HasKey(v => v.Id);

                entity.Property(v => v.VoteType)
                    .HasConversion<int>(); // store enum as int

                entity.HasIndex(v => new { v.SalarySubmissionId, v.UserId })
                    .IsUnique(); // one vote per user per submission
            });
        }
    }
}
