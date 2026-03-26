using LedgerStock.Domain.Common;

namespace LedgerStock.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public int MinimumStock { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();
}