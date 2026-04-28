namespace xablauBack.Application.Contracts.Carrinho;

public interface ICarrinhoService
{
    Task<CarrinhoResponse?> ObterCarrinhoPorUsuarioAsync(int usuarioId); /* pode ser quer retorne null pq talvez não exista carrinho para aquele usuário */
}
