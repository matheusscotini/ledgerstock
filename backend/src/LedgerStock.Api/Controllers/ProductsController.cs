using LedgerStock.Application.DTOs.Products;
using LedgerStock.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    [Authorize(Roles = "Master,Admin")]
    public async Task<ActionResult<ProductResponseDto>> Create([FromBody] CreateProductRequestDto request)
    {
        try
        {
            var product = await _productService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductResponseDto>>> GetAll([FromQuery] string? search, [FromQuery] bool? isActive)
    {
        var products = await _productService.GetAllAsync(search, isActive);
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product is null)
            return NotFound(new { message = "Produto não encontrado." });

        return Ok(product);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Master,Admin")]
    public async Task<ActionResult<ProductResponseDto>> Update(Guid id, [FromBody] UpdateProductRequestDto request)
    {
        try
        {
            var product = await _productService.UpdateAsync(id, request);

            if (product is null)
                return NotFound(new { message = "Produto não encontrado." });

            return Ok(product);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/toggle-status")]
    [Authorize(Roles = "Master,Admin")]
    public async Task<ActionResult<ProductResponseDto>> ToggleStatus(Guid id)
    {
        var product = await _productService.ToggleStatusAsync(id);

        if (product is null)
            return NotFound(new { message = "Produto não encontrado." });

        return Ok(product);
    }
}