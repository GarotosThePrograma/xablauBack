namespace xablau.DTOs.Favorites;

public class FavoriteResponseDto
{
    public int Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
}
