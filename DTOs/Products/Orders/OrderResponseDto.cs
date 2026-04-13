namespace xablau.DTOs.Orders;

public class OrderResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}
