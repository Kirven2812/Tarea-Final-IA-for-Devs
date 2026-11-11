namespace SistemaBiblioteca.API.DTOs
{
    public class CrearUsuarioDto
    {
        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}