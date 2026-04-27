namespace xablauBack.Application.Contracts.Carrinho;

public class CarrinhoResponse
{
    public int CarrinhoId { get; set; }
    public int UsuarioId { get; set; }
    public List<CarrinhoItemResponse> Itens { get; set; } = new(); /* lista de produtos e o "new()" garante que a lista comece vazia */
    public decimal Total { get; set; }
}