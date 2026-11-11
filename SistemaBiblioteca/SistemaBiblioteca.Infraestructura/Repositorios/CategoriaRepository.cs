using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.Aplicacion.Interfaces;
using SistemaBiblioteca.Dominio.Entidades;
using SistemaBiblioteca.Infraestructura.Persistencia;

namespace SistemaBiblioteca.Infraestructura.Repositorios
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly BibliotecaDbContext _context;

        public CategoriaRepository(BibliotecaDbContext context)
        {
            _context = context;
        }

        public async Task<Categoria?> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.NombreCategoria == nombre);
        }

        public async Task AgregarAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
        }
    }
}