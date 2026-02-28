using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechSalaryIdentity.API.DTOs;
using TechSalaryIdentity.Core.Interfaces;

namespace TechSalaryIdentity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCustomers()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return Ok(customers);
    }

    [HttpGet("{customerCode}")]
    public async Task<IActionResult> GetCustomer(string customerCode)
    {
        var customer = await _customerService.GetCustomerByCodeAsync(customerCode);
        if (customer == null)
            return NotFound(new { message = "Customer not found" });

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto createCustomerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var customer = (CustomerResponseDto)await _customerService.CreateCustomerAsync(
                createCustomerDto.CustomerCode,
                createCustomerDto.CustomerName);

            return CreatedAtAction(nameof(GetCustomer), new { customerCode = customer.CustomerCode }, customer);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
