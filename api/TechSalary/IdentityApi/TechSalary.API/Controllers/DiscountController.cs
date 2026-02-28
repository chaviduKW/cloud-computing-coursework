using Microsoft.AspNetCore.Mvc;
using TechSalaryIdentity.API.DTOs;
using TechSalaryIdentity.Core.Interfaces;

namespace TechSalaryIdentity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiscountController : ControllerBase
{
    private readonly IDiscountService _discountService;

    public DiscountController(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDiscounts()
    {
        var discounts = await _discountService.GetAllDiscountsAsync();
        return Ok(discounts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDiscount(int id)
    {
        var discount = await _discountService.GetDiscountByIdAsync(id);
        if (discount == null)
            return NotFound(new { message = "Discount not found" });

        return Ok(discount);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDiscount([FromBody] CreateDiscountDto createDiscountDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var discount = (DiscountResponseDto)await _discountService.CreateDiscountAsync(
                createDiscountDto.DiscountCode,
                createDiscountDto.Description,
                createDiscountDto.DiscountAmount,
                createDiscountDto.DiscountPercentage);

            return CreatedAtAction(nameof(GetDiscount), new { id = discount.Id }, discount);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDiscount(int id, [FromBody] UpdateDiscountDto updateDiscountDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var discount = await _discountService.UpdateDiscountAsync(id,
                updateDiscountDto.Description,
                updateDiscountDto.DiscountAmount,
                updateDiscountDto.DiscountPercentage,
                updateDiscountDto.IsActive);

            if (discount == null)
                return NotFound(new { message = "Discount not found" });

            return Ok(discount);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDiscount(int id)
    {
        var result = await _discountService.DeleteDiscountAsync(id);
        if (!result)
            return NotFound(new { message = "Discount not found" });

        return Ok(new { message = "Discount deleted successfully" });
    }
}
