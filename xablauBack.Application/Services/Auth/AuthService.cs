using System.Security.Cryptography;
using System.Text;
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
            SenhaHash = CriarSenhaHash(request.Senha),
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

    public async Task<LoginResult> LoginAsync(LoginRequest request)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(usuario => usuario.Email == request.Email);

        if (usuario is null)
        {
            return new LoginResult
            {
                Sucesso = false,
                Mensagem = "Email ou senha inválidos"
            };
        }

        var senhaValida = VerificarSenha(request.Senha, usuario.SenhaHash);

        if (!senhaValida)
        {
            return new LoginResult
            {
                Sucesso = false,
                Mensagem = "Email ou senha inválidos"
            };
        }

        return new LoginResult
        {
            Sucesso = true,
            Mensagem = "Login realizado com sucesso",
            Nome = usuario.Nome,
            Email = usuario.Email
        };
    }

    private static string CriarSenhaHash(string senha)
    {
        using var hmac = new HMACSHA512(); /* ferramenta hash */

        var senhaBytes = Encoding.UTF8.GetBytes(senha); /* transforma a senha em uma lista de bytes */
        var hashBytes = hmac.ComputeHash(senhaBytes); /* apartir dos bytes gera o hash de verdade */

        var salt = Convert.ToBase64String(hmac.Key); /*  transforma os bytes em texto para salvar no banco */
        var hash = Convert.ToBase64String(hashBytes); /*  transforma o hash em texto para salvar no banco */

        return $"{salt}.{hash}"; /*  junta o salt e o hash em uma string só */
    }

    private static bool VerificarSenha(string senha, string senhaHashSalva) /* recebe o que o usuário digitou */
    {
        var partes = senhaHashSalva.Split('.'); /* divide os bytes do hash */

        if (partes.Length != 2)
        {
            return false;
        }

        var salt = Convert.FromBase64String(partes[0]); /* transforma o salt(primeira parte) em bytes denovo */
        var hashSalvo = Convert.FromBase64String(partes[1]); /* transforma o hash(segunda parte) em bytes denovo */

        using var hmac = new HMACSHA512(salt); /* recria a ferramenta hash com o mesmo salt do cadastro */

        var senhaBytes = Encoding.UTF8.GetBytes(senha); /* transforma em bytes denovo */
        var hashDigitado = hmac.ComputeHash(senhaBytes); /* transforma o senha em hash com o mesmo salt salvo */

        return CryptographicOperations.FixedTimeEquals(hashDigitado, hashSalvo); /* se o hash antigo bater com o hash atual retun true */
    }
    
}