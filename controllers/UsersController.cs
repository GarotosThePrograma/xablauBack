using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xablau.Data;
using xablau.DTOs.Users;
using xablau.Entities;
using xablau.Enums;

namespace xablau.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        var users = await _context.Users
            .Select(user => ToResponse(user))
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetById(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound("Usuário não encontrado.");
        }

        return Ok(ToResponse(user));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create(CreateUserDto dto)
    {
        if (await _context.Users.AnyAsync(user => user.Email == dto.Email))
        {
            return Conflict("Já existe um usuário com esse email.");
        }

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
        {
            return BadRequest("Role inválida. Use Admin ou User.");
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, ToResponse(user));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponseDto>> Update(int id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound("Usuário não encontrado.");
        }

        if (await _context.Users.AnyAsync(other => other.Email == dto.Email && other.Id != id))
        {
            return Conflict("Já existe outro usuário com esse email.");
        }

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
        {
            return BadRequest("Role inválida. Use Admin ou User.");
        }

        var adminCount = await _context.Users.CountAsync(currentUser => currentUser.Role == UserRole.Admin);
        var isRemovingLastAdminRole = user.Role == UserRole.Admin && role != UserRole.Admin && adminCount == 1;

        if (isRemovingLastAdminRole)
        {
            return BadRequest("Não é possível remover o papel do último administrador.");
        }

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _context.SaveChangesAsync();

        return Ok(ToResponse(user));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound("Usuário não encontrado.");
        }

        var currentUserIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(currentUserIdValue, out var currentUserId) && currentUserId == id)
        {
            return BadRequest("Não é possível remover o próprio usuário autenticado.");
        }

        var adminCount = await _context.Users.CountAsync(currentUser => currentUser.Role == UserRole.Admin);

        if (user.Role == UserRole.Admin && adminCount == 1)
        {
            return BadRequest("Não é possível remover o último administrador.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static UserResponseDto ToResponse(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
