namespace SistemaBiblioteca.API.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }

        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool Estado { get; set; }
        public string Rol { get; set; } = string.Empty;
    }
}