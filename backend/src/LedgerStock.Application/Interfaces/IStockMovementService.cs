using LedgerStock.Application.DTOs.StockMovements;

namespace LedgerStock.Application.Interfaces;

public interface IStockMovementService
{
    Task<StockMovementResponseDto> CreateAsync(CreateStockMovementRequestDto request, string userId);
    Task<List<StockMovementResponseDto>> GetAllAsync(Guid? productId = null, int? type = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<StockMovementResponseDto?> GetByIdAsync(Guid id);
}