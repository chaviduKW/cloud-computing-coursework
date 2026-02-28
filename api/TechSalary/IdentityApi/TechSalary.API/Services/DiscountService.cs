using Microsoft.EntityFrameworkCore;
using TechSalaryIdentity.API.DTOs;
using TechSalaryIdentity.Core.Entities;
using TechSalaryIdentity.Core.Interfaces;
using TechSalaryIdentity.Infrastructure.Data;

namespace TechSalaryIdentity.API.Services;

public class DiscountService : IDiscountService
{
    private readonly AppDbContext _context;

    public DiscountService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<dynamic?> GetDiscountByIdAsync(int id)
    {
        var discount = await _context.Discounts.FindAsync(id);
        return discount == null ? null : MapToDto(discount);
    }

    public async Task<List<dynamic>> GetAllDiscountsAsync()
    {
        var discounts = await _context.Discounts.ToListAsync();
        return discounts.Select<Discount, dynamic>(MapToDto).ToList();
    }

    public async Task<dynamic> CreateDiscountAsync(string discountCode, string description, decimal discountAmount, decimal discountPercentage)
    {
        var discount = new Discount
        {
            DiscountCode = discountCode,
            Description = description,
            DiscountAmount = discountAmount,
            DiscountPercentage = discountPercentage,
            IsActive = true
        };

        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();

        return MapToDto(discount);
    }

    public async Task<dynamic?> UpdateDiscountAsync(int id, string description, decimal discountAmount, decimal discountPercentage, bool isActive)
    {
        var discount = await _context.Discounts.FindAsync(id);
        if (discount == null)
            return null;

        discount.Description = description;
        discount.DiscountAmount = discountAmount;
        discount.DiscountPercentage = discountPercentage;
        discount.IsActive = isActive;
        discount.UpdatedAt = DateTime.UtcNow;

        _context.Discounts.Update(discount);
        await _context.SaveChangesAsync();

        return MapToDto(discount);
    }

    public async Task<bool> DeleteDiscountAsync(int id)
    {
        var discount = await _context.Discounts.FindAsync(id);
        if (discount == null)
            return false;

        _context.Discounts.Remove(discount);
        await _context.SaveChangesAsync();
        return true;
    }

    private DiscountResponseDto MapToDto(Discount discount)
    {
        return new DiscountResponseDto
        {
            Id = discount.Id,
            DiscountCode = discount.DiscountCode,
            Description = discount.Description,
            DiscountAmount = discount.DiscountAmount,
            DiscountPercentage = discount.DiscountPercentage,
            IsActive = discount.IsActive
        };
    }
}
