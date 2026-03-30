using LedgerStock.Application.DTOs.Products;
using LedgerStock.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Globalization;

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

    [HttpGet("export-csv")]
    public async Task<IActionResult> ExportCsv([FromQuery] string? search, [FromQuery] bool? isActive)
    {
        var products = await _productService.GetAllAsync(search, isActive);

        var csv = new StringBuilder();

        csv.AppendLine("Id;Nome;SKU;Descricao;Categoria;Preco;EstoqueMinimo;Ativo;EstoqueAtual;TotalEntradas;TotalSaidas;StatusEstoque;CriadoEm;AtualizadoEm");

        foreach (var product in products)
        {
            csv.AppendLine(string.Join(";",
                EscapeCsv(product.Id.ToString()),
                EscapeCsv(product.Name),
                EscapeCsv(product.Sku),
                EscapeCsv(product.Description),
                EscapeCsv(product.Category),
                EscapeCsv(product.Price.ToString(CultureInfo.InvariantCulture)),
                EscapeCsv(product.MinimumStock.ToString()),
                EscapeCsv(product.IsActive ? "Sim" : "Não"),
                EscapeCsv(product.CurrentStock.ToString()),
                EscapeCsv(product.TotalEntries.ToString()),
                EscapeCsv(product.TotalExits.ToString()),
                EscapeCsv(product.StockStatus),
                EscapeCsv(product.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")),
                EscapeCsv(product.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss"))
            ));
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        var fileName = $"produtos_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        return File(bytes, "text/csv; charset=utf-8", fileName);
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "\"\"";

        var escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }
}