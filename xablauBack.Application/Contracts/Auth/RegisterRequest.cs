/* faz com que o arquivo conheca os arquivos vizinhos */
namespace xablauBack.Application.Contracts.Auth;

/* classe para representar dados de entrada de cadastro */
public class RegisterRequest
{
    public string Nome { get; set; } = string.Empty; /* get e set para ler e alterar valor */
    public string Email { get; set; } = string.Empty; /* comeca vazio pra evitar null */
    public string Senha { get; set; } = string.Empty;
}