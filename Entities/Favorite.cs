namespace xablau.Entities;

public class Favorite
{
    public int Id {get; set; }
    public int UserId {get; set; }
    public Guid ProductId {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public User? User {get; set;}
    public Product? Product {get; set;}
}