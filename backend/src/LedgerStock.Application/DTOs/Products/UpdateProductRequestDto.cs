using System.ComponentModel.DataAnnotations;

namespace LedgerStock.Application.DTOs.Products;

public class UpdateProductRequestDto
{
    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [MinLength(3, ErrorMessage = "O nome do produto deve ter pelo menos 3 caracteres.")]
    [MaxLength(120, ErrorMessage = "O nome do produto deve ter no máximo 120 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O SKU é obrigatório.")]
    [MinLength(3, ErrorMessage = "O SKU deve ter pelo menos 3 caracteres.")]
    [MaxLength(40, ErrorMessage = "O SKU deve ter no máximo 40 caracteres.")]
    public string Sku { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Description { get; set; }

    [MaxLength(80, ErrorMessage = "A categoria deve ter no máximo 80 caracteres.")]
    public string? Category { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O estoque mínimo não pode ser negativo.")]
    public int MinimumStock { get; set; }

    public bool IsActive { get; set; }
}