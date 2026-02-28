namespace TechSalaryIdentity.Core.Entities;

public class Discount
{
    public int Id { get; set; }
    public string DiscountCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercentage { get; set; } = 0; // Optional: for percentage-based discounts
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
