namespace TechSalaryIdentity.Core.Entities;

public class Order
{
    public int OrderId { get; set; } // System generated
    public string CustomerCode { get; set; } = string.Empty;
    public Customer? Customer { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<OrderDetail> OrderDetails { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
