namespace xablauBack.Domain.Entities;

public class Carrinho
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public List<ItemCarrinho> Itens { get; set; } = new();
}