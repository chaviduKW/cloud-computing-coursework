namespace TechSalaryIdentity.API.DTOs;

public class CreateDiscountDto
{
    public string DiscountCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
}

public class UpdateDiscountDto
{
    public string Description { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsActive { get; set; }
}

public class DiscountResponseDto
{
    public int Id { get; set; }
    public string DiscountCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsActive { get; set; }
}
