namespace LedgerStock.Application.DTOs.StockMovements;

public class StockMovementResponseDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public int Type { get; set; }
    public string TypeLabel { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string PerformedByUserId { get; set; } = string.Empty;
    public string PerformedByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}