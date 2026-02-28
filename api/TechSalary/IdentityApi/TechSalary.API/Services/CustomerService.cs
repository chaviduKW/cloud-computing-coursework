using Microsoft.EntityFrameworkCore;
using TechSalaryIdentity.API.DTOs;
using TechSalaryIdentity.Core.Entities;
using TechSalaryIdentity.Core.Interfaces;
using TechSalaryIdentity.Infrastructure.Data;

namespace TechSalaryIdentity.API.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<dynamic?> GetCustomerByCodeAsync(string customerCode)
    {
        var customer = await _context.Customers.FindAsync(customerCode);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<List<dynamic>> GetAllCustomersAsync()
    {
        var customers = await _context.Customers.ToListAsync();
        return customers.Select<Customer, dynamic>(MapToDto).ToList();
    }

    public async Task<dynamic> CreateCustomerAsync(string customerCode, string customerName)
    {
        var customer = new Customer
        {
            CustomerCode = customerCode,
            CustomerName = customerName
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return MapToDto(customer);
    }

    private CustomerResponseDto MapToDto(Customer customer)
    {
        return new CustomerResponseDto
        {
            CustomerCode = customer.CustomerCode,
            CustomerName = customer.CustomerName
        };
    }
}
