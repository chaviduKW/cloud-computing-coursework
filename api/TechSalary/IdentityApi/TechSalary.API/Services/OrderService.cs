using Microsoft.EntityFrameworkCore;
using TechSalaryIdentity.API.DTOs;
using TechSalaryIdentity.Core.Entities;
using TechSalaryIdentity.Core.Interfaces;
using TechSalaryIdentity.Infrastructure.Data;

namespace TechSalaryIdentity.API.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<dynamic?> GetOrderByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        return order == null ? null : MapToDto(order);
    }

    public async Task<List<dynamic>> GetAllOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
            .Include(o => o.Customer)
            .ToListAsync();

        return orders.Select<Order, dynamic>(MapToDto).ToList();
    }

    public async Task<dynamic> CreateOrderAsync(string customerCode, decimal taxRate, string? notes, List<(string productCode, int quantity, decimal unitPrice, decimal discount)> orderDetails, string createdBy)
    {
        // Validate customer exists
        var customer = await _context.Customers.FindAsync(customerCode);
        if (customer == null)
            throw new InvalidOperationException($"Customer with code {customerCode} not found");

        // Calculate order totals
        var details = new List<OrderDetail>();
        decimal subTotal = 0;
        decimal discountTotal = 0;

        foreach (var detail in orderDetails)
        {
            var itemTotal = (detail.quantity * detail.unitPrice) - detail.discount;
            subTotal += detail.quantity * detail.unitPrice;
            discountTotal += detail.discount;

            details.Add(new OrderDetail
            {
                ProductCode = detail.productCode,
                Quantity = detail.quantity,
                UnitPrice = detail.unitPrice,
                Discount = detail.discount,
                Total = itemTotal
            });
        }

        // Calculate grand total
        var taxableAmount = subTotal - discountTotal;
        var taxAmount = (taxableAmount * taxRate) / 100;
        var grandTotal = taxableAmount + taxAmount;

        var order = new Order
        {
            CustomerCode = customerCode,
            Customer = customer,
            SubTotal = subTotal,
            DiscountTotal = discountTotal,
            TaxRate = taxRate,
            GrandTotal = grandTotal,
            CreatedBy = createdBy,
            Notes = notes,
            OrderDetails = details
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return MapToDto(order);
    }

    public async Task<dynamic?> UpdateOrderAsync(int id, string customerCode, decimal taxRate, string? notes, List<(string productCode, int quantity, decimal unitPrice, decimal discount)> orderDetails)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return null;

        // Validate customer exists
        var customer = await _context.Customers.FindAsync(customerCode);
        if (customer == null)
            throw new InvalidOperationException($"Customer with code {customerCode} not found");

        // Remove old order details
        _context.OrderDetails.RemoveRange(order.OrderDetails);

        // Calculate new totals
        var details = new List<OrderDetail>();
        decimal subTotal = 0;
        decimal discountTotal = 0;

        foreach (var detail in orderDetails)
        {
            var itemTotal = (detail.quantity * detail.unitPrice) - detail.discount;
            subTotal += detail.quantity * detail.unitPrice;
            discountTotal += detail.discount;

            details.Add(new OrderDetail
            {
                OrderId = id,
                ProductCode = detail.productCode,
                Quantity = detail.quantity,
                UnitPrice = detail.unitPrice,
                Discount = detail.discount,
                Total = itemTotal
            });
        }

        // Calculate grand total
        var taxableAmount = subTotal - discountTotal;
        var taxAmount = (taxableAmount * taxRate) / 100;
        var grandTotal = taxableAmount + taxAmount;

        order.CustomerCode = customerCode;
        order.Customer = customer;
        order.SubTotal = subTotal;
        order.DiscountTotal = discountTotal;
        order.TaxRate = taxRate;
        order.GrandTotal = grandTotal;
        order.Notes = notes;
        order.OrderDetails = details;
        order.UpdatedAt = DateTime.UtcNow;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return MapToDto(order);
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    private OrderResponseDto MapToDto(Order order)
    {
        return new OrderResponseDto
        {
            OrderId = order.OrderId,
            CustomerCode = order.CustomerCode,
            CustomerName = order.Customer?.CustomerName ?? string.Empty,
            SubTotal = order.SubTotal,
            DiscountTotal = order.DiscountTotal,
            TaxRate = order.TaxRate,
            GrandTotal = order.GrandTotal,
            OrderDate = order.OrderDate,
            CreatedBy = order.CreatedBy,
            Notes = order.Notes,
            OrderDetails = order.OrderDetails.Select(d => new OrderDetailResponseDto
            {
                Id = d.Id,
                ProductCode = d.ProductCode,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Discount = d.Discount,
                Total = d.Total
            }).ToList()
        };
    }
}
