using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xablau.Data;
using xablau.DTOs.Favorites;
using xablau.Entities;

namespace xablau.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly AppDbContext _context;

    public FavoritesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FavoriteResponseDto>>> GetAll()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var favorites = await _context.Favorites
            .Where(favorite => favorite.UserId == userId.Value)
            .Include(favorite => favorite.Product)
            .Select(favorite => new FavoriteResponseDto
            {
                Id = favorite.Id,
                ProductId = favorite.ProductId,
                ProductName = favorite.Product!.Name,
                Description = favorite.Product.Description,
                Price = favorite.Product.Price,
                ImageUrl = favorite.Product.ImageUrl,
                Stock = favorite.Product.Stock,
                CreatedAt = favorite.CreatedAt
            })
            .ToListAsync();

        return Ok(favorites);
    }

    [HttpPost("{productId}")]
    public async Task<ActionResult<FavoriteResponseDto>> Add(Guid productId)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var product = await _context.Products.FindAsync(productId);

        if (product is null)
        {
            return NotFound("Produto não encontrado.");
        }

        var alreadyExists = await _context.Favorites
            .AnyAsync(favorite => favorite.UserId == userId.Value && favorite.ProductId == productId);

        if (alreadyExists)
        {
            return Conflict("Esse produto já está nos favoritos.");
        }

        var favorite = new Favorite
        {
            UserId = userId.Value,
            ProductId = productId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        var response = new FavoriteResponseDto
        {
            Id = favorite.Id,
            ProductId = product.Id,
            ProductName = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Stock = product.Stock,
            CreatedAt = favorite.CreatedAt
        };

        return CreatedAtAction(nameof(GetAll), response);
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> Remove(Guid productId)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(favorite => favorite.UserId == userId.Value && favorite.ProductId == productId);

        if (favorite is null)
        {
            return NotFound("Favorito não encontrado.");
        }

        _context.Favorites.Remove(favorite);
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
}
