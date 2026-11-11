using SistemaBiblioteca.Aplicacion.Modelos;
using SistemaBiblioteca.Dominio.Entidades;
using SistemaBiblioteca.Dominio.Enums;

namespace SistemaBiblioteca.Aplicacion.Interfaces
{
    public interface ILibroRepository
    {
        Task<Libro?> ObtenerPorIdAsync(int id);

        Task<Libro?> ObtenerPorIsbnAsync(string isbn);

        Task<List<Libro>> ObtenerTodosAsync();

        Task AgregarAsync(Libro libro);

        void Eliminar(Libro libro);

        Task GuardarCambiosAsync();
        Task<ResultadoPaginado<Libro>> BuscarAsync(
            string? titulo,
            string? autor,
            string? categoria,
            EstadoLibro? estado,
            int pagina,
            int tamañoPagina);
    }
}