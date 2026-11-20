namespace SistemaBiblioteca.Web.Models
{
    public class PrestamoModel
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public string UsuarioNombre { get; set; } = string.Empty;

        public int LibroId { get; set; }

        public string LibroTitulo { get; set; } = string.Empty;

        public DateTime FechaPrestamo { get; set; }

        public DateTime? FechaDevolucion { get; set; }

        public string Estado { get; set; } = string.Empty;
    }
}
