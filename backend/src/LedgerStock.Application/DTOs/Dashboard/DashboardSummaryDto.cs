namespace LedgerStock.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int InactiveProducts { get; set; }
    public int TotalMovements { get; set; }
    public int TotalEntries { get; set; }
    public int TotalExits { get; set; }
    public int ProductsWithLowStock { get; set; }
    public int ProductsOutOfStock { get; set; }
}