namespace xablauBack.Application.Contracts.Carrinho;

public interface ICarrinhoService
{
    Task<CarrinhoResponse?> ObterCarrinhoPorUsuarioAsync(int usuarioId); /* pode ser quer retorne null pq talvez não exista carrinho para aquele usuário */
    Task<CarrinhoResponse?> AdicionarItemAsync(int usuarioId, AdicionarItemCarrinhoRequest request);
    Task<CarrinhoResponse?> RemoverItemAsync(int usuarioId, int produtoId);
    Task<CarrinhoResponse?> AtualizarQuantidadeAsync(int usuarioId, int produtoId, AtualizarQuantidadeItemRequest request);
    Task<CarrinhoResponse?> LimparCarrinhoAsync(int usuarioId);

}
