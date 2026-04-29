/* faz com que o arquivo conheca os arquivos vizinhos */
namespace xablauBack.Application.Contracts.Auth;

public interface IAuthService /* todo serviço de auth precisa saber cadsatrar e logar  */
{
    Task<RegisterResult> RegisterAsync (RegisterRequest request);
    Task<LoginResult> LoginAsync (LoginRequest request);
}
/* define que todo servico de autenticacao precisa ter um RegisterAsync */

