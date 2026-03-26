using System.Security.Claims;
using LedgerStock.Application.DTOs.StockMovements;
using LedgerStock.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockMovementsController : ControllerBase
{
    private readonly IStockMovementService _stockMovementService;

    public StockMovementsController(IStockMovementService stockMovementService)
    {
        _stockMovementService = stockMovementService;
    }

    [HttpPost]
    [Authorize(Roles = "Master,Admin")]
    public async Task<ActionResult<StockMovementResponseDto>> Create([FromBody] CreateStockMovementRequestDto request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var movement = await _stockMovementService.CreateAsync(request, userId);

            return CreatedAtAction(nameof(GetById), new { id = movement.Id }, movement);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<StockMovementResponseDto>>> GetAll(
        [FromQuery] Guid? productId,
        [FromQuery] int? type,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var movements = await _stockMovementService.GetAllAsync(productId, type, startDate, endDate);
        return Ok(movements);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StockMovementResponseDto>> GetById(Guid id)
    {
        var movement = await _stockMovementService.GetByIdAsync(id);

        if (movement is null)
            return NotFound(new { message = "Movimentação não encontrada." });

        return Ok(movement);
    }
}