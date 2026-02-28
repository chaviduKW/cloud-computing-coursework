using Microsoft.EntityFrameworkCore;
using TechSalaryIdentity.Core.Entities;

namespace TechSalaryIdentity.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer - hard coded seed data with CustomerCode as PK
        modelBuilder.Entity<Customer>()
            .HasKey(c => c.CustomerCode);

        modelBuilder.Entity<Customer>().HasData(
            new Customer { CustomerCode = "C001", CustomerName = "John Doe Enterprises", CreatedAt = DateTime.UtcNow },
            new Customer { CustomerCode = "C002", CustomerName = "Lanka Traders Pvt Ltd", CreatedAt = DateTime.UtcNow },
            new Customer { CustomerCode = "C003", CustomerName = "ABC Solutions", CreatedAt = DateTime.UtcNow }
        );

        // Order -> OrderDetail relationship
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderDetails)
            .WithOne(d => d.Order)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Order -> Customer relationship
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerCode)
            .HasPrincipalKey(c => c.CustomerCode);

        // Decimal precision for monetary values
        modelBuilder.Entity<Product>()
            .Property(p => p.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Discount>()
            .Property(d => d.DiscountAmount)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<Discount>()
            .Property(d => d.DiscountPercentage)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.SubTotal)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<Order>()
            .Property(o => o.DiscountTotal)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<Order>()
            .Property(o => o.TaxRate)
            .HasPrecision(5, 2);
        
        modelBuilder.Entity<Order>()
            .Property(o => o.GrandTotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderDetail>()
            .Property(d => d.UnitPrice)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<OrderDetail>()
            .Property(d => d.Discount)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<OrderDetail>()
            .Property(d => d.Total)
            .HasPrecision(18, 2);
    }
}
