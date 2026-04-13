namespace xablau.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Cart? Cart { get; set; }
    public Product? Product { get; set; }
}
