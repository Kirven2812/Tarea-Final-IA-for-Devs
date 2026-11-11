using SistemaBiblioteca.Dominio.Enums;
namespace SistemaBiblioteca.Dominio.Entidades
{
    public class Usuario
    {
        public int Id { get; private set; }

        public string Nombres { get; private set; }

        public string Apellidos { get; private set; }

        public string Email { get; private set; }

        public string PasswordHash { get; private set; }

        public bool Estado { get; private set; }

        private readonly List<Prestamo> _prestamos = new();

        public IReadOnlyCollection<Prestamo> Prestamos => _prestamos;
        public RolUsuario Rol { get; private set; }

        private Usuario()
        {
        }

        public Usuario(
            string nombres,
            string apellidos,
            string email,
            string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(nombres))
                throw new ArgumentException(
                    "Los nombres son obligatorios.",
                    nameof(nombres));

            if (string.IsNullOrWhiteSpace(apellidos))
                throw new ArgumentException(
                    "Los apellidos son obligatorios.",
                    nameof(apellidos));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "El email es obligatorio.",
                    nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException(
                    "El password es obligatorio.",
                    nameof(passwordHash));

            Nombres = nombres;
            Apellidos = apellidos;
            Email = email;
            PasswordHash = passwordHash;

            Estado = true;
            Rol = RolUsuario.Usuario;
        }

        public void Activar()
        {
            Estado = true;
        }

        public void Desactivar()
        {
            Estado = false;
        }

        public void CambiarPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException(
                    "El password hash es obligatorio.",
                    nameof(passwordHash));
            }

            PasswordHash = passwordHash;
        }
        public void CambiarRol(RolUsuario nuevoRol)
        {
            Rol = nuevoRol;
        }
    }
}