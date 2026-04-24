/* faz com que o arquivo conheca os arquivos vizinhos */
namespace xablauBack.Application.Contracts.Auth;

public class RegisterResult
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}