using System.Security.Cryptography.X509Certificates;
using xablauBack.Application.Contracts.Auth;

namespace xablauBack.Application.Services.Auth;

public class AuthService : IAuthService
{
    public Task<RegisterResult> RegisterAsync(RegisterRequest request)
    {
        public Task<RegisterResult> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
            {
                return Task.FromResult(new RegisterResult 
                {
                    Sucesso = false,
                    Mensagem = "Nome obrigatório"
                });
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Task.FromResult(new RegisterResult
                {
                    Sucesso = false,
                    Mensagem = "Email obrigatório"
                });
            }
            
            if (string.IsNullOrWhiteSpace(request.Senha))
            {
                return Task.FromResult(new RegisterResult
                {
                    Sucesso = true,
                    Mensagem = "Senha obrigatória"
                });      
            }

            
            return Task.FromResult(new RegisterResult
            {
                Sucesso = true,
                Mensagem = "Cadastro válido"
            });      
            
        }
    }
}