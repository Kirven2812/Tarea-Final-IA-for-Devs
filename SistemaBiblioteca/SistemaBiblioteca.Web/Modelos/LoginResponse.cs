namespace SistemaBiblioteca.Web.Models
{
    public class LoginResponse
    {
        public UsuarioModel Usuario { get; set; } = new();

        public string Token { get; set; } = string.Empty;
    }
}