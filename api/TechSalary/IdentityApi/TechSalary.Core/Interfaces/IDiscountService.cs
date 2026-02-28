namespace TechSalaryIdentity.Core.Interfaces;

public interface IDiscountService
{
    Task<dynamic?> GetDiscountByIdAsync(int id);
    Task<List<dynamic>> GetAllDiscountsAsync();
    Task<dynamic> CreateDiscountAsync(string discountCode, string description, decimal discountAmount, decimal discountPercentage);
    Task<dynamic?> UpdateDiscountAsync(int id, string description, decimal discountAmount, decimal discountPercentage, bool isActive);
    Task<bool> DeleteDiscountAsync(int id);
}
