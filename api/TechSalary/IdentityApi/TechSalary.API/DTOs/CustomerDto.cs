namespace TechSalaryIdentity.API.DTOs;

public class CustomerResponseDto
{
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}

public class CreateCustomerDto
{
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}
