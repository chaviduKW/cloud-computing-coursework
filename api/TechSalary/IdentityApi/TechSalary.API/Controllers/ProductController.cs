using Microsoft.AspNetCore.Mvc;
using TechSalaryIdentity.API.DTOs;
using TechSalaryIdentity.Core.Interfaces;

namespace TechSalaryIdentity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound(new { message = "Product not found" });

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var product = (ProductResponseDto)await _productService.CreateProductAsync(
                createProductDto.ProductCode,
                createProductDto.ProductName,
                createProductDto.UnitPrice);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var product = await _productService.UpdateProductAsync(id,
                updateProductDto.ProductName,
                updateProductDto.UnitPrice,
                updateProductDto.IsActive);

            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result)
            return NotFound(new { message = "Product not found" });

        return Ok(new { message = "Product deleted successfully" });
    }
}
