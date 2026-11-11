using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.Aplicacion.Interfaces;
using SistemaBiblioteca.Dominio.Entidades;
using SistemaBiblioteca.Infraestructura.Persistencia;

namespace SistemaBiblioteca.Infraestructura.Repositorios
{
    public class PrestamoRepository : IPrestamoRepository
    {
        private readonly BibliotecaDbContext _context;

        public PrestamoRepository(BibliotecaDbContext context)
        {
            _context = context;
        }

        public async Task<Prestamo?> ObtenerPorIdAsync(int id)
        {
            return await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AgregarAsync(Prestamo prestamo)
        {
            await _context.Prestamos.AddAsync(prestamo);
        }

        public async Task GuardarCambiosAsync()
        {
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                Console.WriteLine(
                    $"Entidad: {entry.Entity.GetType().Name} | Estado: {entry.State}");
            }

            await _context.SaveChangesAsync();
        }
        public async Task<List<Prestamo>> ObtenerTodosAsync()
        {
            return await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                .ToListAsync();
        }
        public async Task<List<Prestamo>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();
        }

    }
}