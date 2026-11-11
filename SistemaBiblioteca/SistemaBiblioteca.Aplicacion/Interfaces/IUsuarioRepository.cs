using SistemaBiblioteca.Dominio.Entidades;

namespace SistemaBiblioteca.Aplicacion.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorIdAsync(int id);

        Task<Usuario?> ObtenerPorEmailAsync(string email);

        Task<IEnumerable<Usuario>> ObtenerTodosAsync();

        Task AgregarAsync(Usuario usuario);

        Task GuardarCambiosAsync();
        Task<Usuario?> ObtenerConPrestamosAsync(int id);
    }
}