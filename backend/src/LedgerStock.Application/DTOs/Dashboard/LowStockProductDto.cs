namespace LedgerStock.Application.DTOs.Dashboard;

public class LowStockProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public bool IsOutOfStock { get; set; }
    public string StockStatus { get; set; } = string.Empty;
}