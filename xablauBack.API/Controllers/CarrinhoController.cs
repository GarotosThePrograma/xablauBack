using Microsoft.AspNetCore.Mvc;
using xablauBack.Application.Contracts.Carrinho;

namespace xablauBack.API.Controller;

[ApiController]
[Route("api/[controller]")] /* /api/carrinho */
public class CarrinhoController : ControllerBase
{
    private readonly ICarrinhoService _carrinhoService;

    public CarrinhoController(ICarrinhoService carrinhoService)
    {
        _carrinhoService = carrinhoService;
    }

    [HttpGet("{usuarioId:int}")] /* GET /api/carrinho/1 (1 seria o id do usuário) */
    public async Task<IActionResult> ObterCarrinho(int usuarioId)
    {
        var carrinho = await _carrinhoService.ObterCarrinhoPorUsuarioAsync(usuarioId); /* chama o sevice pra buscar o carrinho daquele usuário */

        if(carrinho is null)
        {
            return NotFound("Carrinho não encontrado para esse usuário"); /* se não achar carrinho 404 */
        }

        return Ok(carrinho); /* se achar 200 */
    }

    [HttpPost("{usuarioId:int}/itens")] /* POST /api/carrinho/1/itens */
    public async Task<IActionResult> AdicionarItem(int usuarioId, [FromBody] AdicionarItemCarrinhoRequest request)
    {
    var carrinho = await _carrinhoService.AdicionarItemAsync(usuarioId, request);

    if (carrinho is null)
    {
        return BadRequest("Não foi possível adicionar o item ao carrinho");
    }

    return Ok(carrinho);
    }

    [HttpDelete("{usuarioId:int}/itens/{produtoId:int}")] /* DELETE /api/carrinho/{id do usuário}/itens/{id do item do usuário} */
    public async Task<IActionResult> RemoverItem(int usuarioId, int produtoId)
    {
        var carrinho = await _carrinhoService.RemoverItemAsync(usuarioId, produtoId);

        if (carrinho is null)
        {
            return NotFound("Item não encontrado no carrinho");
        }

        return Ok(carrinho);
    }

    /* PUT(UPDATE) /api/carrinho/{id do usuário}/itens/{id do item do usuário} */
    [HttpPut("{usuarioId:int}/itens/{produtoId:int}")]
    public async Task<IActionResult> AtualizarQuantidade(int usuarioId, int produtoId, [FromBody] AtualizarQuantidadeItemRequest request)
    {
        var carrinho = await _carrinhoService.AtualizarQuantidadeAsync(usuarioId, produtoId, request);

        if (carrinho is null)
        {
            return NotFound("Item não encontrado no carrinho");
        }

        return Ok(carrinho);
    }


}