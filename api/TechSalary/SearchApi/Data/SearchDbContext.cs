using Microsoft.EntityFrameworkCore;
using SearchApi.Entities;

namespace SearchApi.Data
{
    public class SearchDbContext(DbContextOptions<SearchDbContext> options) : DbContext(options)
    {
        public DbSet<SalaryRecord> SalaryRecords => Set<SalaryRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SalaryRecord>(entity =>
            {
                entity.ToTable("salary_records");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.CompanyName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Designation).HasMaxLength(200).IsRequired();
                entity.Property(e => e.SalaryAmount).HasPrecision(18, 2);
                entity.Property(e => e.Currency).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.ExperienceLevel).HasMaxLength(50);

                // Read-optimized indexes for search and sort operations
                entity.HasIndex(e => e.CompanyName);
                entity.HasIndex(e => e.Designation);
                entity.HasIndex(e => e.SalaryAmount);
                entity.HasIndex(e => e.SubmittedAt);
                entity.HasIndex(e => e.TotalVotes);
                entity.HasIndex(e => e.Location);
                entity.HasIndex(e => e.ExperienceLevel);

                // Composite index for the most common search combo
                entity.HasIndex(e => new { e.CompanyName, e.Designation });
            });
        }
    }
}
