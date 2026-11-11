using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.Aplicacion.Interfaces;
using SistemaBiblioteca.Aplicacion.Modelos;
using SistemaBiblioteca.Dominio.Entidades;
using SistemaBiblioteca.Dominio.Enums;
using SistemaBiblioteca.Infraestructura.Persistencia;

namespace SistemaBiblioteca.Infraestructura.Repositorios
{
    public class LibroRepository : ILibroRepository
    {
        private readonly BibliotecaDbContext _context;

        public LibroRepository(BibliotecaDbContext context)
        {
            _context = context;
        }

        public async Task<Libro?> ObtenerPorIdAsync(int id)
        {
            return await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
        public async Task<Libro?> ObtenerPorIsbnAsync(string isbn)
        {
            return await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(l => l.Isbn == isbn);
        }

        public async Task<List<Libro>> ObtenerTodosAsync()
        {
            return await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .ToListAsync();
        }

        public async Task AgregarAsync(Libro libro)
        {
            await _context.Libros.AddAsync(libro);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void Eliminar(Libro libro)
        {
            _context.Libros.Remove(libro);
        }
        public async Task<ResultadoPaginado<Libro>> BuscarAsync(
            string? titulo,
            string? autor,
            string? categoria,
            EstadoLibro? estado,
            int pagina,
            int tamañoPagina)
        {
            var query = _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(titulo))
            {
                query = query.Where(l =>
                    l.Titulo.Contains(titulo));
            }

            if (!string.IsNullOrWhiteSpace(autor))
            {
                query = query.Where(l =>
                    l.Autor.NombreAutor.Contains(autor));
            }

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                query = query.Where(l =>
                    l.Categoria.NombreCategoria.Contains(categoria));
            }

            if (estado.HasValue)
            {
                query = query.Where(l =>
                    l.Estado == estado.Value);
            }

            var totalRegistros =
                await query.CountAsync();

            var libros = await query
                .OrderBy(l => l.Titulo)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToListAsync();

            return new ResultadoPaginado<Libro>
            {
                Items = libros,
                Pagina = pagina,
                TamañoPagina = tamañoPagina,
                TotalRegistros = totalRegistros
            };
        }
    }
}