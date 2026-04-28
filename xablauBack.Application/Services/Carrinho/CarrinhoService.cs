using Microsoft.EntityFrameworkCore;
using xablauBack.Application.Contracts.Carrinho;
using xablauBack.Domain.Entities;
using xablauBack.Infrastructure.Data;

namespace xablauBack.Application.Services.Carrinho;

public class CarrinhoService : ICarrinhoService
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

    public async Task<CarrinhoResponse?> AdicionarItemAsync(int usuarioId, AdicionarItemCarrinhoRequest request)
    {
        if (request.Quantidade <= 0) /* bloqueia quantidade inválida */
        {
            return null;
        }

        var carrinho = await _context.Carrinhos
            .Include(carrinho => carrinho.Itens) /* busca o carrinho com os itens que ele já tem */
            .FirstOrDefaultAsync(carrinho => carrinho.UsuarioId == usuarioId); /* confere se bate com o usuário */

        if (carrinho is null)
        {
            return null;
        }
        
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(produto => produto.Id == request.ProdutoId);

        if (produto is null)
        {
            return null;
        }

        /* verifica se o produto já está no carrinho, se não existe cria novo, se já existe soma */
        var itemExistente = carrinho.Itens
            .FirstOrDefault(itemExistente => itemExistente.ProdutoId == request.ProdutoId);

        if (itemExistente is null)
        {
            var novoItem = new ItemCarrinho
            {
                CarrinhoId = carrinho.Id,
                ProdutoId = produto.Id,
                Quantidade = request.Quantidade
            };

            _context.ItensCarrinho.Add(novoItem);
        }
        else
        {
            itemExistente.Quantidade += request.Quantidade;
        }

        await _context.SaveChangesAsync();

        return await ObterCarrinhoPorUsuarioAsync(usuarioId); /* retorna carrinho atualizado */
    }   
}