namespace TechSalaryIdentity.Core.Entities;

public class Customer
{
    public string CustomerCode { get; set; } = string.Empty; // PK, hard-coded master table
    public string CustomerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
