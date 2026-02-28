namespace TechSalaryIdentity.Core.Interfaces;

public interface IOrderService
{
    Task<dynamic?> GetOrderByIdAsync(int id);
    Task<List<dynamic>> GetAllOrdersAsync();
    Task<dynamic> CreateOrderAsync(string customerCode, decimal taxRate, string? notes, List<(string productCode, int quantity, decimal unitPrice, decimal discount)> orderDetails, string createdBy);
    Task<dynamic?> UpdateOrderAsync(int id, string customerCode, decimal taxRate, string? notes, List<(string productCode, int quantity, decimal unitPrice, decimal discount)> orderDetails);
    Task<bool> DeleteOrderAsync(int id);
}
