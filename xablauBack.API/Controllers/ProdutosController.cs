using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xablauBack.Infrastructure.Data;

namespace xablauBack.API.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context; /* acessa a tabela de produtos */
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var produtos = await _context.Produtos
            .OrderBy(produto => produto.Id) /* ordena por id */
            .Select(produto => new /* transforma o produto do banco no formato que o front espera */
            {
                id = produto.Id,
                name = produto.Nome,
                price = produto.Preco,
                stock = produto.Estoque,
                img = produto.ImagemUrl,
            })
            .ToListAsync(); 

        return Ok(produtos);
    }
}