namespace LedgerStock.Application.DTOs.Products;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public int MinimumStock { get; set; }
    public bool IsActive { get; set; }
    public int CurrentStock { get; set; }
    public int TotalEntries { get; set; }
    public int TotalExits { get; set; }
    public string StockStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}