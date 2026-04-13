using System.ComponentModel.DataAnnotations;

namespace xablau.DTOs.Cart;

public class UpdateCartItemDto
{
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
