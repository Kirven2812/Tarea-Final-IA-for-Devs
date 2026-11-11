using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.Aplicacion.Interfaces;
using SistemaBiblioteca.Dominio.Entidades;
using SistemaBiblioteca.Infraestructura.Persistencia;

namespace SistemaBiblioteca.Infraestructura.Repositorios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly BibliotecaDbContext _context;

        public UsuarioRepository(BibliotecaDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios
                .ToListAsync();
        }

        public async Task AgregarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> ObtenerConPrestamosAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Prestamos)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}