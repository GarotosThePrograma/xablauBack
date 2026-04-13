using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xablau.Data;
using xablau.DTOs.Cart;
using xablau.Entities;

namespace xablau.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<CartResponseDto>> GetCart()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var cart = await GetOrCreateCartAsync(userId.Value);
        return Ok(ToResponse(cart));
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartResponseDto>> AddItem(AddCartItemDto dto)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var product = await _context.Products.FindAsync(dto.ProductId);

        if (product is null)
        {
            return NotFound("Produto não encontrado.");
        }

        if (dto.Quantity > product.Stock)
        {
            return BadRequest("Quantidade solicitada excede o estoque disponível.");
        }

        var cart = await GetOrCreateCartAsync(userId.Value);

        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(item => item.CartId == cart.Id && item.ProductId == dto.ProductId);

        if (existingItem is not null)
        {
            var newQuantity = existingItem.Quantity + dto.Quantity;

            if (newQuantity > product.Stock)
            {
                return BadRequest("Quantidade solicitada excede o estoque disponível.");
            }

            existingItem.Quantity = newQuantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _context.CartItems.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var updatedCart = await LoadCartAsync(userId.Value);
        return Ok(ToResponse(updatedCart!));
    }

    [HttpPut("items/{itemId}")]
    public async Task<ActionResult<CartResponseDto>> UpdateItem(int itemId, UpdateCartItemDto dto)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var item = await _context.CartItems
            .Include(cartItem => cartItem.Cart)
            .Include(cartItem => cartItem.Product)
            .FirstOrDefaultAsync(cartItem => cartItem.Id == itemId && cartItem.Cart!.UserId == userId.Value);

        if (item is null)
        {
            return NotFound("Item do carrinho não encontrado.");
        }

        if (dto.Quantity > item.Product!.Stock)
        {
            return BadRequest("Quantidade solicitada excede o estoque disponível.");
        }

        item.Quantity = dto.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        item.Cart!.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var updatedCart = await LoadCartAsync(userId.Value);
        return Ok(ToResponse(updatedCart!));
    }

    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> RemoveItem(int itemId)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var item = await _context.CartItems
            .Include(cartItem => cartItem.Cart)
            .FirstOrDefaultAsync(cartItem => cartItem.Id == itemId && cartItem.Cart!.UserId == userId.Value);

        if (item is null)
        {
            return NotFound("Item do carrinho não encontrado.");
        }

        item.Cart!.UpdatedAt = DateTime.UtcNow;

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(userIdValue, out var userId))
        {
            return userId;
        }

        return null;
    }

    private async Task<Cart> GetOrCreateCartAsync(int userId)
    {
        var cart = await LoadCartAsync(userId);

        if (cart is not null)
        {
            return cart;
        }

        cart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        return await LoadCartAsync(userId) ?? cart;
    }

    private async Task<Cart?> LoadCartAsync(int userId)
    {
        return await _context.Carts
            .Include(cart => cart.Items)
            .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(cart => cart.UserId == userId);
    }

    private static CartResponseDto ToResponse(Cart cart)
    {
        var items = cart.Items.Select(item => new CartItemResponseDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product?.Name ?? string.Empty,
            ImageUrl = item.Product?.ImageUrl ?? string.Empty,
            UnitPrice = item.Product?.Price ?? 0,
            Quantity = item.Quantity,
            Subtotal = (item.Product?.Price ?? 0) * item.Quantity
        }).ToList();

        return new CartResponseDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            UpdatedAt = cart.UpdatedAt,
            TotalAmount = items.Sum(item => item.Subtotal),
            Items = items
        };
    }
}
