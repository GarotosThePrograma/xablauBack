using Microsoft.EntityFrameworkCore;
using xablauBack.Application.Contracts.Auth;
using xablauBack.Domain.Entities;
using xablauBack.Infrastructure.Data;

namespace xablauBack.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context; /* variavel privada */

    public AuthService(AppDbContext context)
    {
        _context = context; /* guarda o banco aqui */
    }

    public async Task<RegisterResult> RegisterAsync(RegisterRequest request) /* método de cadastro */
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return new RegisterResult
            {
                Sucesso = false,
                Mensagem =  "Nome obrigatório"
            };
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return new RegisterResult
            {
                Sucesso = false,
                Mensagem = "Email obrigatório"
            };
        }
        
        if (string.IsNullOrWhiteSpace(request.Senha))
        {
            return new RegisterResult
            {
                Sucesso = false,
                Mensagem = "Senha obrigatória"
            };      
        }

        var emailJaExiste = await _context.Usuarios /* _context.Usuarios representa a tabela de usuários do banco */
            .AnyAsync(usuario => usuario.Email == request.Email); /* verifica no banco se algum usuário tem o mesmo email */
        
        if(emailJaExiste)
        {
            return new RegisterResult
            {
                Sucesso = false,
                Mensagem = "Email já cadastrado"
            };
        }

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = request.Senha,
        };

        var carrinho = new Carrinho
        {
            Usuario = usuario /* este carrinho pertence a esse usuário */
        };

        _context.Usuarios.Add(usuario);
        _context.Carrinhos.Add(carrinho);

        await _context.SaveChangesAsync(); /* o INSERT do sql acontece aqui */

        return new RegisterResult
        {
            Sucesso = true,
            Mensagem = "Cadastro realizado com sucesso"
        };
    }
}