using LedgerStock.Application.DTOs.Products;
using LedgerStock.Application.Interfaces;
using LedgerStock.Domain.Entities;
using LedgerStock.Domain.Enums;
using LedgerStock.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LedgerStock.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductRequestDto request)
    {
        var normalizedSku = request.Sku.Trim().ToUpper();

        var skuAlreadyExists = await _context.Products
            .AnyAsync(p => p.Sku.ToUpper() == normalizedSku);

        if (skuAlreadyExists)
            throw new InvalidOperationException("Já existe um produto com este SKU.");

        var product = new Product
        {
            Name = request.Name.Trim(),
            Sku = normalizedSku,
            Description = request.Description?.Trim(),
            Category = request.Category?.Trim(),
            Price = request.Price,
            MinimumStock = request.MinimumStock,
            IsActive = true
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return await MapToResponseDto(product.Id);
    }

    public async Task<List<ProductResponseDto>> GetAllAsync(string? search = null, bool? isActive = null)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Movements)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();

            query = query.Where(p =>
                p.Name.ToLower().Contains(normalizedSearch) ||
                p.Sku.ToLower().Contains(normalizedSearch) ||
                (p.Category != null && p.Category.ToLower().Contains(normalizedSearch)));
        }

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        var products = await query
            .OrderBy(p => p.Name)
            .ToListAsync();

        return products.Select(MapToResponseDto).ToList();
    }

    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Movements)
            .FirstOrDefaultAsync(p => p.Id == id);

        return product is null ? null : MapToResponseDto(product);
    }

    public async Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductRequestDto request)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return null;

        var normalizedSku = request.Sku.Trim().ToUpper();

        var skuAlreadyExists = await _context.Products
            .AnyAsync(p => p.Id != id && p.Sku.ToUpper() == normalizedSku);

        if (skuAlreadyExists)
            throw new InvalidOperationException("Já existe outro produto com este SKU.");

        product.Name = request.Name.Trim();
        product.Sku = normalizedSku;
        product.Description = request.Description?.Trim();
        product.Category = request.Category?.Trim();
        product.Price = request.Price;
        product.MinimumStock = request.MinimumStock;
        product.IsActive = request.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await MapToResponseDto(product.Id);
    }

    public async Task<ProductResponseDto?> ToggleStatusAsync(Guid id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return null;

        product.IsActive = !product.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await MapToResponseDto(product.Id);
    }

    private async Task<ProductResponseDto> MapToResponseDto(Guid productId)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Movements)
            .FirstAsync(p => p.Id == productId);

        return MapToResponseDto(product);
    }

    private static ProductResponseDto MapToResponseDto(Product product)
    {
        var totalEntries = product.Movements
            .Where(m => m.Type == MovementType.Entry)
            .Sum(m => m.Quantity);

        var totalExits = product.Movements
            .Where(m => m.Type == MovementType.Exit)
            .Sum(m => m.Quantity);

        var currentStock = totalEntries - totalExits;

        var stockStatus = currentStock switch
        {
            <= 0 => "Sem estoque",
            _ when currentStock <= product.MinimumStock => "Estoque baixo",
            _ => "Estoque normal"
        };

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Description = product.Description,
            Category = product.Category,
            Price = product.Price,
            MinimumStock = product.MinimumStock,
            IsActive = product.IsActive,
            CurrentStock = currentStock,
            TotalEntries = totalEntries,
            TotalExits = totalExits,
            StockStatus = stockStatus,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}