namespace TechSalaryIdentity.API.DTOs;

public class CreateOrderDto
{
    public string CustomerCode { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public string? Notes { get; set; }
    public List<CreateOrderDetailDto> OrderDetails { get; set; } = new();
}

public class UpdateOrderDto
{
    public string CustomerCode { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public string? Notes { get; set; }
    public List<CreateOrderDetailDto> OrderDetails { get; set; } = new();
}

public class CreateOrderDetailDto
{
    public string ProductCode { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
}

public class OrderResponseDto
{
    public int OrderId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime OrderDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<OrderDetailResponseDto> OrderDetails { get; set; } = new();
}

public class OrderDetailResponseDto
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
}
