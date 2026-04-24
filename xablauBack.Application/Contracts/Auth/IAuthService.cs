/* faz com que o arquivo conheca os arquivos vizinhos */
namespace xablauBack.Application.Contracts.Auth;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync (RegisterRequest request);
}
/* define que todo servico de autenticacao precisa ter um RegisterAsync */

