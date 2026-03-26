using LedgerStock.Application.DTOs.Dashboard;

namespace LedgerStock.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();
    Task<List<LowStockProductDto>> GetLowStockProductsAsync();
}