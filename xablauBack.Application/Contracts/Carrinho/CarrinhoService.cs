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
        var carrinho = await _context.Carrinhos
            .Include(carrinho => Carrinho.Itens)
            .ThenInclude(ItemCarrinho => ItemCarrinho.produto)
            .FirstOrDefaultAsync(carrinho => carrinho.UsuarioId == usuarioId);

        if (carrinho is null)
        {
            return null;
        }

        var response = new CarrinhoResponse
        {
            CarrinhoId = carrinho.Id,
            UsuarioId = carrinho.UsuarioId,
            Itens = carrinho.Itens.Select(item => new CarrinhoItemResponse
            {
                ProdutoId = item.ProdutoId,
                Nome = item.Produto.Nome,
                ImagemUrl = item.Produto.ImagemUrl,
                Preco = item.Produto.Preco,
                Quantidade = item.Quantidade,
                Subtotal = item.Produto.Preco * item.Quantidade
            })
        }
    }
}