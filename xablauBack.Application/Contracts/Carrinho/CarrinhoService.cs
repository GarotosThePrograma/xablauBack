using Microsoft.EntityFrameworkCore;
using xablauBack.Application.Contracts.Carrinho;
using xablauBack.Domain.Entities;
using xablauBack.Infrastructure.Data;

namespace xablauBack.Infrastructure.Data;

public class Carrinho : ICarrinhoService
{
    private readonly AppDbContext _context;

    public CarrinhoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CarrinhoResponse?> ObterCarrinhoPorUsuarioAsync(int usuarioId)
    {
        var carrinho = await _context.Carrinhos /* acessa a tablela de carrinhos */
            .Include(carrinho => carrinho.Itens) /* faz o EF trazer os itens do carrinho */
            .ThenInclude(itemCarrinho => itemCarrinho.Produto) /* carrega cada produto de cada item */
            .FirstOrDefaultAsync(carrinho => carrinho.UsuarioId == usuarioId); /* busca o primeiro carrinho daquele usuário */

        if (carrinho is null) /* se não achar o carrinho retorna null */
        {
            return null;
        }

        var response = new CarrinhoResponse
        {
            CarrinhoId = carrinho.Id,
            UsuarioId = carrinho.UsuarioId,
            Itens = carrinho.Itens.Select(item => new CarrinhoItemResponse /* organizando o que vai para o banco */
            {
                ProdutoId = item.ProdutoId,
                Nome = item.Produto.Nome,
                ImagemUrl = item.Produto.ImagemUrl,
                Preco = item.Produto.Preco,
                Quantidade = item.Quantidade,
                Subtotal = item.Produto.Preco * item.Quantidade
            }).ToList()
        };

        response.Total = response.Itens.Sum(item => item.Subtotal); /* soma tudo */

        return response;
    }
}