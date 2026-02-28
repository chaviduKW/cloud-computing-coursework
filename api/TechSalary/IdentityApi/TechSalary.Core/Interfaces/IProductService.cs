namespace TechSalaryIdentity.Core.Interfaces;

public interface IProductService
{
    Task<dynamic?> GetProductByIdAsync(int id);
    Task<List<dynamic>> GetAllProductsAsync();
    Task<dynamic> CreateProductAsync(string productCode, string productName, decimal unitPrice);
    Task<dynamic?> UpdateProductAsync(int id, string productName, decimal unitPrice, bool isActive);
    Task<bool> DeleteProductAsync(int id);
}
