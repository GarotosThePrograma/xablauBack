using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xablau.Data;
using xablau.DTOs.Orders;
using xablau.Entities;

namespace xablau.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<OrderResponseDto>> Checkout()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var cart = await _context.Carts
            .Include(currentCart => currentCart.Items)
            .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(currentCart => currentCart.UserId == userId.Value);

        if (cart is null || !cart.Items.Any())
        {
            return BadRequest("O carrinho está vazio.");
        }

        foreach (var item in cart.Items)
        {
            if (item.Product is null)
            {
                return BadRequest("Um dos produtos do carrinho não foi encontrado.");
            }

            if (item.Quantity > item.Product.Stock)
            {
                return BadRequest($"Estoque insuficiente para o produto {item.Product.Name}.");
            }
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var order = new Order
        {
            UserId = userId.Value,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var cartItem in cart.Items)
        {
            var product = cartItem.Product!;

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductNameSnapshot = product.Name,
                UnitPrice = product.Price,
                Quantity = cartItem.Quantity,
                Subtotal = product.Price * cartItem.Quantity
            });

            product.Stock -= cartItem.Quantity;
        }

        order.TotalAmount = order.Items.Sum(item => item.Subtotal);

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cart.Items);

        cart.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        var savedOrder = await _context.Orders
            .Include(currentOrder => currentOrder.Items)
            .FirstAsync(currentOrder => currentOrder.Id == order.Id);

        return CreatedAtAction(nameof(GetById), new { id = savedOrder.Id }, ToResponse(savedOrder));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAll()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var orders = await _context.Orders
            .Where(order => order.UserId == userId.Value)
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync();

        return Ok(orders.Select(ToResponse));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var order = await _context.Orders
            .Include(currentOrder => currentOrder.Items)
            .FirstOrDefaultAsync(currentOrder => currentOrder.Id == id && currentOrder.UserId == userId.Value);

        if (order is null)
        {
            return NotFound("Pedido não encontrado.");
        }

        return Ok(ToResponse(order));
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

    private static OrderResponseDto ToResponse(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(item => new OrderItemResponseDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductNameSnapshot,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                Subtotal = item.Subtotal
            }).ToList()
        };
    }
}
