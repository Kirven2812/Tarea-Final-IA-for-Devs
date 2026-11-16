using SistemaBiblioteca.Dominio.Entidades;

namespace SistemaBiblioteca.Aplicacion.Interfaces
{
    public interface IAutorRepository
    {
        Task<Autor?> ObtenerPorNombreAsync(string nombre);

        Task AgregarAsync(Autor autor);
    }
}