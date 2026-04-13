namespace xablau.DTOs.Cart;

public class CartResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public List<CartItemResponseDto> Items { get; set; } = new();
}
