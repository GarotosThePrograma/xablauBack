namespace xablauBack.Application.Contracts.Auth;

public class LoginResult
{
    public bool Sucesso { get; set; }  /* verifica se o login deu certo */
    public string Mensagem { get; set; } = string.Empty;
    public int? UsuarioId { get; set; } /* não pode ser nulo */
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}