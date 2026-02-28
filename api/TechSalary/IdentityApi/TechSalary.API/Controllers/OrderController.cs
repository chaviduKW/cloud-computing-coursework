using Microsoft.AspNetCore.Mvc;
using TechSalaryIdentity.API.DTOs;
using TechSalaryIdentity.Core.Interfaces;

namespace TechSalaryIdentity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound(new { message = "Order not found" });

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdBy = "System";
            
            var orderDetails = createOrderDto.OrderDetails
                .Select(d => (d.ProductCode, d.Quantity, d.UnitPrice, d.Discount))
                .ToList();

            var order = (OrderResponseDto)await _orderService.CreateOrderAsync(
                createOrderDto.CustomerCode,
                createOrderDto.TaxRate,
                createOrderDto.Notes,
                orderDetails,
                createdBy);

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var orderDetails = updateOrderDto.OrderDetails
                .Select(d => (d.ProductCode, d.Quantity, d.UnitPrice, d.Discount))
                .ToList();

            var order = await _orderService.UpdateOrderAsync(id,
                updateOrderDto.CustomerCode,
                updateOrderDto.TaxRate,
                updateOrderDto.Notes,
                orderDetails);

            if (order == null)
                return NotFound(new { message = "Order not found" });

            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var result = await _orderService.DeleteOrderAsync(id);
        if (!result)
            return NotFound(new { message = "Order not found" });

        return Ok(new { message = "Order deleted successfully" });
    }
}
