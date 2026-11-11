using SistemaBiblioteca.Dominio.Entidades;

namespace SistemaBiblioteca.Aplicacion.Interfaces
{
    public interface IPrestamoRepository
    {
        Task<Prestamo?> ObtenerPorIdAsync(int id);

        Task<List<Prestamo>> ObtenerTodosAsync();

        Task<List<Prestamo>> ObtenerPorUsuarioAsync(int usuarioId);

        Task AgregarAsync(Prestamo prestamo);

        Task GuardarCambiosAsync();
    }
}