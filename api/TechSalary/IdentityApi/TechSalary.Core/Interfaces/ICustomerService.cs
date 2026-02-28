namespace TechSalaryIdentity.Core.Interfaces;

public interface ICustomerService
{
    Task<dynamic?> GetCustomerByCodeAsync(string customerCode);
    Task<List<dynamic>> GetAllCustomersAsync();
    Task<dynamic> CreateCustomerAsync(string customerCode, string customerName);
}
