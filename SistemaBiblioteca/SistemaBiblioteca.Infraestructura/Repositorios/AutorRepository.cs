using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.Aplicacion.Interfaces;
using SistemaBiblioteca.Dominio.Entidades;
using SistemaBiblioteca.Infraestructura.Persistencia;

namespace SistemaBiblioteca.Infraestructura.Repositorios
{
    public class AutorRepository : IAutorRepository
    {
        private readonly BibliotecaDbContext _context;

        public AutorRepository(BibliotecaDbContext context)
        {
            _context = context;
        }

        public async Task<Autor?> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.Autores
                .FirstOrDefaultAsync(a => a.NombreAutor == nombre);
        }

        public async Task AgregarAsync(Autor autor)
        {
            await _context.Autores.AddAsync(autor);
        }
    }
}