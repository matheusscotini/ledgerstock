using System.ComponentModel.DataAnnotations;

namespace LedgerStock.Application.DTOs.StockMovements;

public class CreateStockMovementRequestDto
{
    [Required(ErrorMessage = "O produto é obrigatório.")]
    public Guid ProductId { get; set; }

    [Range(1, 2, ErrorMessage = "O tipo de movimentação deve ser 1 (Entrada) ou 2 (Saída).")]
    public int Type { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "O motivo da movimentação é obrigatório.")]
    [MinLength(2, ErrorMessage = "O motivo deve ter pelo menos 2 caracteres.")]
    [MaxLength(100, ErrorMessage = "O motivo deve ter no máximo 100 caracteres.")]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "A observação deve ter no máximo 500 caracteres.")]
    public string? Notes { get; set; }
}