using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xablau.Data;
using xablau.DTOs.Products;
using xablau.Entities;

namespace xablau.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll()
    {
        var products = await _context.Products
            .Select(product => new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Stock = product.Stock,
                CreatedAt = product.CreatedAt
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return NotFound("Produto não encontrado.");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }



    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create(CreateProductDto dto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrl = dto.ImageUrl,
            Stock = dto.Stock,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var response = new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult<ProductResponseDto>> Update(Guid id, CreateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return NotFound("Produto não encontrado.");
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.ImageUrl = dto.ImageUrl;
        product.Stock = dto.Stock;

        await _context.SaveChangesAsync();

        var response = new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
    {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Produto não encontrado.");
            }
        

        var response = new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt
        };

        return Ok(response);
    }


}