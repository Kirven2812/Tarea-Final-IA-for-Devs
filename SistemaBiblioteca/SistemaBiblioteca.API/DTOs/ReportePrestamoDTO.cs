
namespace SistemaBiblioteca.API.DTOs
{
    public class ReportePrestamoDTO
    {
        public int IdPrestamo { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Libro { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public DateTime FechaPrestamo { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}