namespace TechSalaryIdentity.API.DTOs;

public class CreateProductDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
}

public class UpdateProductDto
{
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public bool IsActive { get; set; }
}

public class ProductResponseDto
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
