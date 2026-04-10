using System.ComponentModel.DataAnnotations;

namespace xablau.DTOs.Products;

public class CreateProductDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}
