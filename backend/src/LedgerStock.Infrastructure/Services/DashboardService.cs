using LedgerStock.Application.DTOs.Dashboard;
using LedgerStock.Application.Interfaces;
using LedgerStock.Domain.Enums;
using LedgerStock.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LedgerStock.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Movements)
            .ToListAsync();

        var totalProducts = products.Count;
        var activeProducts = products.Count(p => p.IsActive);
        var inactiveProducts = products.Count(p => !p.IsActive);

        var totalEntries = products.Sum(p => p.Movements
            .Where(m => m.Type == MovementType.Entry)
            .Sum(m => m.Quantity));

        var totalExits = products.Sum(p => p.Movements
            .Where(m => m.Type == MovementType.Exit)
            .Sum(m => m.Quantity));

        var totalMovements = products.Sum(p => p.Movements.Count);

        var lowStockCount = 0;
        var outOfStockCount = 0;

        foreach (var product in products)
        {
            var entries = product.Movements
                .Where(m => m.Type == MovementType.Entry)
                .Sum(m => m.Quantity);

            var exits = product.Movements
                .Where(m => m.Type == MovementType.Exit)
                .Sum(m => m.Quantity);

            var currentStock = entries - exits;

            if (currentStock <= 0)
            {
                outOfStockCount++;
            }
            else if (currentStock <= product.MinimumStock)
            {
                lowStockCount++;
            }
        }

        return new DashboardSummaryDto
        {
            TotalProducts = totalProducts,
            ActiveProducts = activeProducts,
            InactiveProducts = inactiveProducts,
            TotalMovements = totalMovements,
            TotalEntries = totalEntries,
            TotalExits = totalExits,
            ProductsWithLowStock = lowStockCount,
            ProductsOutOfStock = outOfStockCount
        };
    }

    public async Task<List<LowStockProductDto>> GetLowStockProductsAsync()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Movements)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

        var result = new List<LowStockProductDto>();

        foreach (var product in products)
        {
            var totalEntries = product.Movements
                .Where(m => m.Type == MovementType.Entry)
                .Sum(m => m.Quantity);

            var totalExits = product.Movements
                .Where(m => m.Type == MovementType.Exit)
                .Sum(m => m.Quantity);

            var currentStock = totalEntries - totalExits;

            if (currentStock <= product.MinimumStock)
            {
                result.Add(new LowStockProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Sku = product.Sku,
                    Category = product.Category,
                    CurrentStock = currentStock,
                    MinimumStock = product.MinimumStock,
                    IsOutOfStock = currentStock <= 0,
                    StockStatus = currentStock <= 0 ? "Sem estoque" : "Estoque baixo"
                });
            }
        }

        return result;
    }
}