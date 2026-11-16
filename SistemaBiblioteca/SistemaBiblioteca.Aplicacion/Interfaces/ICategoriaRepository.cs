using SistemaBiblioteca.Dominio.Entidades;

namespace SistemaBiblioteca.Aplicacion.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<Categoria?> ObtenerPorNombreAsync(string nombre);

        Task AgregarAsync(Categoria categoria);
    }
}