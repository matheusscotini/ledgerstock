using LedgerStock.Application.DTOs.Products;

namespace LedgerStock.Application.Interfaces;

public interface IProductService
{
    Task<ProductResponseDto> CreateAsync(CreateProductRequestDto request);
    Task<List<ProductResponseDto>> GetAllAsync(string? search = null, bool? isActive = null);
    Task<ProductResponseDto?> GetByIdAsync(Guid id);
    Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductRequestDto request);
    Task<ProductResponseDto?> ToggleStatusAsync(Guid id);
}