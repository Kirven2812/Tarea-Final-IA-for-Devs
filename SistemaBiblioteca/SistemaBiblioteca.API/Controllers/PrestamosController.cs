using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.API.DTOs;
using SistemaBiblioteca.Dominio.Entidades;
using SistemaBiblioteca.Infraestructura.Persistencia;
using System.Security.Claims;

namespace SistemaBiblioteca.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamosController : ControllerBase
    {
        private readonly PrestamoService _prestamoService;

        public PrestamosController(
            PrestamoService prestamoService, BibliotecaDbContext context)
        {
            _prestamoService = prestamoService;
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CrearPrestamo(
            CrearPrestamoDto dto)
        {
            try
            {
                var usuarioIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier);

                if (usuarioIdClaim == null)
                    return Unauthorized();

                if (!int.TryParse(
                    usuarioIdClaim.Value,
                    out int usuarioId))
                {
                    return Unauthorized();
                }

                var prestamo =
                    await _prestamoService.PrestarLibroAsync(
                        usuarioId,
                        dto.LibroId);

                return Ok(new
                {
                    prestamo.Id,
                    prestamo.UsuarioId,
                    prestamo.LibroId,
                    prestamo.FechaPrestamo,
                    prestamo.Estado
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPut("{id:int}/devolver")]
        public async Task<IActionResult> DevolverLibro(int id)
        {
            try
            {
                var usuarioIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier);

                if (usuarioIdClaim == null)
                    return Unauthorized();

                if (!int.TryParse(
                    usuarioIdClaim.Value,
                    out int usuarioId))
                {
                    return Unauthorized();
                }

                var prestamo =
                    await _prestamoService.DevolverLibroAsync(
                        id,
                        usuarioId);

                return Ok(new
                {
                    prestamo.Id,
                    prestamo.UsuarioId,
                    prestamo.LibroId,
                    prestamo.FechaPrestamo,
                    prestamo.FechaDevolucion,
                    prestamo.Estado
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrestamoDto>>> GetPrestamos()
        {
            var prestamos =
                await _prestamoService.ObtenerTodosAsync();

            var resultado = prestamos.Select(MapearADto);

            return Ok(resultado);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PrestamoDto>> GetPrestamo(int id)
        {
            var prestamo =
                await _prestamoService.ObtenerPorIdAsync(id);

            if (prestamo == null)
                return NotFound();

            return Ok(MapearADto(prestamo));
        }

        [Authorize]
        [HttpGet("mis-prestamos")]
        public async Task<ActionResult<IEnumerable<PrestamoDto>>> MisPrestamos()
        {
            try
            {
                var usuarioIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier);

                if (usuarioIdClaim == null)
                    return Unauthorized();

                if (!int.TryParse(
                    usuarioIdClaim.Value,
                    out int usuarioId))
                {
                    return Unauthorized();
                }

                var prestamos =
                    await _prestamoService
                        .ObtenerPorUsuarioAsync(usuarioId);

                var resultado =
                    prestamos.Select(MapearADto);

                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        private static PrestamoDto MapearADto(Prestamo prestamo)
        {
            return new PrestamoDto
            {
                Id = prestamo.Id,
                UsuarioId = prestamo.UsuarioId,
                UsuarioNombre =
                    $"{prestamo.Usuario.Nombres} {prestamo.Usuario.Apellidos}",
                LibroId = prestamo.LibroId,
                LibroTitulo = prestamo.Libro.Titulo,
                FechaPrestamo = prestamo.FechaPrestamo,
                FechaDevolucion = prestamo.FechaDevolucion,
                Estado = prestamo.Estado.ToString()
            };
        }





        
        private readonly BibliotecaDbContext _context; // <-- Cambia por el nombre de tu DbContext
        /*
        public PrestamosController(BibliotecaDbContext context, )
        {
            _context = context;
        }
        */

        [HttpGet("reporte")]
        public async Task<ActionResult<IEnumerable<ReportePrestamoDTO>>> GetReportePrestamos(
                    [FromQuery] DateTime? fechaInicio,
                    [FromQuery] DateTime? fechaFin,
                    [FromQuery] string? autor,
                    [FromQuery] string? nombreLibro)
        {
            // Consulta uniendo las 4 tablas: Prestamos -> Usuarios, Libros -> Autores
            //_prestamoService
            
            var query = _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                    .ThenInclude(l => l.Autor)
                .AsQueryable();
            
            // 1. Filtro por Fecha Inicio
            if (fechaInicio.HasValue)
            {
                query = query.Where(p => p.FechaPrestamo.Date >= fechaInicio.Value.Date);
            }

            // 2. Filtro por Fecha Fin
            if (fechaFin.HasValue)
            {
                query = query.Where(p => p.FechaPrestamo.Date <= fechaFin.Value.Date);
            }

            // 3. Filtro por Autor
            if (!string.IsNullOrWhiteSpace(autor))
            {
                query = query.Where(p => p.Libro.Autor.NombreAutor.Contains(autor));
            }

            // 4. Filtro por Nombre del Libro
            if (!string.IsNullOrWhiteSpace(nombreLibro))
            {
                query = query.Where(p => p.Libro.Titulo.Contains(nombreLibro));
            }

            var resultado = await query
                .OrderByDescending(p => p.FechaPrestamo)
                .Select(p => new ReportePrestamoDTO
                {
                    //IdPrestamo = p.IdPrestamo,
                    Usuario = p.Usuario.Nombres, // Ajustar si tienes Nombre + Apellido
                    Libro = p.Libro.Titulo,
                    Autor = p.Libro.Autor.NombreAutor,
                    FechaPrestamo = p.FechaPrestamo,
                    FechaDevolucion = p.FechaDevolucion,
                    Estado = p.FechaDevolucion == null ? "Pendiente" : "Devuelto"
                })
                .ToListAsync();

            return Ok(resultado);
        }



    }
}