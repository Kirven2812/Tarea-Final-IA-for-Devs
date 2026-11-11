using Microsoft.AspNetCore.Mvc;
using SistemaBiblioteca.API.DTOs;
using SistemaBiblioteca.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SistemaBiblioteca.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamosController : ControllerBase
    {
        private readonly PrestamoService _prestamoService;

        public PrestamosController(
            PrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
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
    }
}