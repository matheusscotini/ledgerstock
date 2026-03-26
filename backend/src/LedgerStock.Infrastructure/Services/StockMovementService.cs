using LedgerStock.Application.DTOs.StockMovements;
using LedgerStock.Application.Interfaces;
using LedgerStock.Domain.Entities;
using LedgerStock.Domain.Enums;
using LedgerStock.Infrastructure.Identity;
using LedgerStock.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LedgerStock.Infrastructure.Services;

public class StockMovementService : IStockMovementService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StockMovementService(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<StockMovementResponseDto> CreateAsync(CreateStockMovementRequestDto request, string userId)
    {
        if (!Enum.IsDefined(typeof(MovementType), request.Type))
            throw new InvalidOperationException("Tipo de movimentação inválido.");

        if (request.Quantity <= 0)
            throw new InvalidOperationException("A quantidade deve ser maior que zero.");

        var product = await _context.Products
            .Include(p => p.Movements)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId);

        if (product is null)
            throw new InvalidOperationException("Produto não encontrado.");

        if (!product.IsActive)
            throw new InvalidOperationException("Não é permitido movimentar um produto inativo.");

        var movementType = (MovementType)request.Type;

        var totalEntries = product.Movements
            .Where(m => m.Type == MovementType.Entry)
            .Sum(m => m.Quantity);

        var totalExits = product.Movements
            .Where(m => m.Type == MovementType.Exit)
            .Sum(m => m.Quantity);

        var currentStock = totalEntries - totalExits;

        if (movementType == MovementType.Exit && request.Quantity > currentStock)
            throw new InvalidOperationException("Estoque insuficiente para realizar a saída.");

        var movement = new StockMovement
        {
            ProductId = request.ProductId,
            Type = movementType,
            Quantity = request.Quantity,
            Reason = request.Reason.Trim(),
            Notes = request.Notes?.Trim(),
            PerformedByUserId = userId
        };

        _context.StockMovements.Add(movement);
        await _context.SaveChangesAsync();

        return await GetMovementResponseAsync(movement.Id);
    }

    public async Task<List<StockMovementResponseDto>> GetAllAsync(
        Guid? productId = null,
        int? type = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _context.StockMovements
            .AsNoTracking()
            .Include(m => m.Product)
            .AsQueryable();

        if (productId.HasValue)
            query = query.Where(m => m.ProductId == productId.Value);

        if (type.HasValue)
            query = query.Where(m => (int)m.Type == type.Value);

        if (startDate.HasValue)
            query = query.Where(m => m.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
        {
            var inclusiveEndDate = endDate.Value.Date.AddDays(1);
            query = query.Where(m => m.CreatedAt < inclusiveEndDate);
        }

        var movements = await query
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        var userIds = movements
            .Select(m => m.PerformedByUserId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();

        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FullName })
            .ToListAsync();

        var usersDictionary = users.ToDictionary(u => u.Id, u => u.FullName);

        return movements.Select(m => new StockMovementResponseDto
        {
            Id = m.Id,
            ProductId = m.ProductId,
            ProductName = m.Product.Name,
            ProductSku = m.Product.Sku,
            Type = (int)m.Type,
            TypeLabel = m.Type == MovementType.Entry ? "Entrada" : "Saída",
            Quantity = m.Quantity,
            Reason = m.Reason,
            Notes = m.Notes,
            PerformedByUserId = m.PerformedByUserId,
            PerformedByUserName = usersDictionary.TryGetValue(m.PerformedByUserId, out var userName)
                ? userName
                : "Usuário não encontrado",
            CreatedAt = m.CreatedAt
        }).ToList();
    }

    public async Task<StockMovementResponseDto?> GetByIdAsync(Guid id)
    {
        var movement = await _context.StockMovements
            .AsNoTracking()
            .Include(m => m.Product)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movement is null)
            return null;

        var user = await _context.Users
            .Where(u => u.Id == movement.PerformedByUserId)
            .Select(u => new { u.FullName })
            .FirstOrDefaultAsync();

        return new StockMovementResponseDto
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            ProductName = movement.Product.Name,
            ProductSku = movement.Product.Sku,
            Type = (int)movement.Type,
            TypeLabel = movement.Type == MovementType.Entry ? "Entrada" : "Saída",
            Quantity = movement.Quantity,
            Reason = movement.Reason,
            Notes = movement.Notes,
            PerformedByUserId = movement.PerformedByUserId,
            PerformedByUserName = user?.FullName ?? "Usuário não encontrado",
            CreatedAt = movement.CreatedAt
        };
    }

    private async Task<StockMovementResponseDto> GetMovementResponseAsync(Guid movementId)
    {
        var movement = await _context.StockMovements
            .AsNoTracking()
            .Include(m => m.Product)
            .FirstAsync(m => m.Id == movementId);

        var user = await _context.Users
            .Where(u => u.Id == movement.PerformedByUserId)
            .Select(u => new { u.FullName })
            .FirstOrDefaultAsync();

        return new StockMovementResponseDto
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            ProductName = movement.Product.Name,
            ProductSku = movement.Product.Sku,
            Type = (int)movement.Type,
            TypeLabel = movement.Type == MovementType.Entry ? "Entrada" : "Saída",
            Quantity = movement.Quantity,
            Reason = movement.Reason,
            Notes = movement.Notes,
            PerformedByUserId = movement.PerformedByUserId,
            PerformedByUserName = user?.FullName ?? "Usuário não encontrado",
            CreatedAt = movement.CreatedAt
        };
    }
}