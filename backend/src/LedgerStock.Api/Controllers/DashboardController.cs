using LedgerStock.Application.DTOs.Dashboard;
using LedgerStock.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
    {
        var summary = await _dashboardService.GetSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("low-stock-products")]
    public async Task<ActionResult<List<LowStockProductDto>>> GetLowStockProducts()
    {
        var products = await _dashboardService.GetLowStockProductsAsync();
        return Ok(products);
    }
}