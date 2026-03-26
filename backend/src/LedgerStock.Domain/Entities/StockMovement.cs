using LedgerStock.Domain.Common;
using LedgerStock.Domain.Enums;

namespace LedgerStock.Domain.Entities;

public class StockMovement : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public MovementType Type { get; set; }
    public int Quantity { get; set; }

    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public string PerformedByUserId { get; set; } = string.Empty;
}