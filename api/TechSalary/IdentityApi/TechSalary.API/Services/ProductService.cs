using Microsoft.EntityFrameworkCore;
using TechSalaryIdentity.API.DTOs;
using TechSalaryIdentity.Core.Entities;
using TechSalaryIdentity.Core.Interfaces;
using TechSalaryIdentity.Infrastructure.Data;

namespace TechSalaryIdentity.API.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<dynamic?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<List<dynamic>> GetAllProductsAsync()
    {
        var products = await _context.Products.ToListAsync();
        return products.Select<Product, dynamic>(MapToDto).ToList();
    }

    public async Task<dynamic> CreateProductAsync(string productCode, string productName, decimal unitPrice)
    {
        var product = new Product
        {
            ProductCode = productCode,
            ProductName = productName,
            UnitPrice = unitPrice,
            IsActive = true
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task<dynamic?> UpdateProductAsync(int id, string productName, decimal unitPrice, bool isActive)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return null;

        product.ProductName = productName;
        product.UnitPrice = unitPrice;
        product.IsActive = isActive;
        product.UpdatedAt = DateTime.UtcNow;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    private ProductResponseDto MapToDto(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            UnitPrice = product.UnitPrice,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
    }
}
