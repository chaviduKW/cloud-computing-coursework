namespace TechSalaryIdentity.Core.Entities;

public class OrderDetail
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; } // (Qty * UnitPrice) - Discount
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
