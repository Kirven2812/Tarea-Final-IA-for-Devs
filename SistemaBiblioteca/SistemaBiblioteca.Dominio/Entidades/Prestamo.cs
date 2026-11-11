using SistemaBiblioteca.Dominio.Enums;

namespace SistemaBiblioteca.Dominio.Entidades
{
    public class Prestamo
    {
        public int Id { get; private set; }

        public int UsuarioId { get; private set; }

        public int LibroId { get; private set; }

        public Usuario Usuario { get; private set; }

        public Libro Libro { get; private set; }

        public DateTime FechaPrestamo { get; private set; }

        public DateTime? FechaDevolucion { get; private set; }

        public EstadoPrestamo Estado { get; private set; }

        private Prestamo()
        {
        }

        private Prestamo(Usuario usuario, Libro libro)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            if (libro == null)
                throw new ArgumentNullException(nameof(libro));

            if (!usuario.Estado)
                throw new InvalidOperationException(
                    "El usuario está inactivo.");

            libro.Prestar();

            Usuario = usuario;
            Libro = libro;
            FechaPrestamo = DateTime.UtcNow;
            Estado = EstadoPrestamo.Activo;
        }

        public static Prestamo Crear(
            Usuario usuario,
            Libro libro)
        {
            return new Prestamo(usuario, libro);
        }

        public void Devolver()
        {
            if (Estado == EstadoPrestamo.Devuelto)
                throw new InvalidOperationException(
                    "El préstamo ya fue devuelto.");

            Libro.Devolver();

            FechaDevolucion = DateTime.UtcNow;
            Estado = EstadoPrestamo.Devuelto;
        }
    }
}