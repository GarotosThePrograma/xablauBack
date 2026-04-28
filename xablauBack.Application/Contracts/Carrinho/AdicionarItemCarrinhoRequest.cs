namespace xablauBack.Application.Contracts.Carrinho;

public class AdicionarItemCarrinhoRequest
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}