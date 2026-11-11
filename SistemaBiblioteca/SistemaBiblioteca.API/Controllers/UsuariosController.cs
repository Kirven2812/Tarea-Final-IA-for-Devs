using Microsoft.AspNetCore.Mvc;
using SistemaBiblioteca.API.DTOs;
using SistemaBiblioteca.Aplicacion.Interfaces;
using SistemaBiblioteca.Aplicacion.Servicios;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SistemaBiblioteca.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly IJwtTokenService _jwtTokenService;

        public UsuariosController(
            UsuarioService usuarioService,
            IJwtTokenService jwtTokenService)
        {
            _usuarioService = usuarioService;
            _jwtTokenService = jwtTokenService;
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();

            var resultado = usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombres = u.Nombres,
                Apellidos = u.Apellidos,
                Email = u.Email,
                Estado = u.Estado,
                Rol = u.Rol.ToString()
            });

            return Ok(resultado);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(int id)
        {
            var usuario = await _usuarioService.ObtenerPorIdAsync(id);

            if (usuario == null)
                return NotFound();

            var dto = new UsuarioDto
            {
                Id = usuario.Id,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Email = usuario.Email,
                Estado = usuario.Estado,
                Rol = usuario.Rol.ToString()
            };

            return Ok(dto);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> CrearUsuario(
            CrearUsuarioDto dto)
        {
            try
            {
                var usuario = await _usuarioService.CrearAsync(
                    dto.Nombres,
                    dto.Apellidos,
                    dto.Email,
                    dto.Password);

                var resultado = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nombres = usuario.Nombres,
                    Apellidos = usuario.Apellidos,
                    Email = usuario.Email,
                    Estado = usuario.Estado,
                    Rol = usuario.Rol.ToString()
                };

                return CreatedAtAction(
                    nameof(GetUsuario),
                    new { id = usuario.Id },
                    resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(
            LoginDto dto)
        {
            try
            {
                var usuario = await _usuarioService.LoginAsync(
                    dto.Email,
                    dto.Password);

                if (usuario == null)
                {
                    return Unauthorized(new
                    {
                        mensaje = "Email o contraseña incorrectos."
                    });
                }

                var token = _jwtTokenService.GenerarToken(usuario);

                var resultado = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nombres = usuario.Nombres,
                    Apellidos = usuario.Apellidos,
                    Email = usuario.Email,
                    Estado = usuario.Estado,
                    Rol = usuario.Rol.ToString()
                };

                return Ok(new
                {
                    usuario = resultado,
                    token = token
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
        [HttpPut("{id:int}/password")]
        public async Task<IActionResult> CambiarPassword(
            int id,
            CambiarPasswordDto dto)
        {
            try
            {
                await _usuarioService.CambiarPasswordAsync(
                    id,
                    dto.NuevaPassword);

                return NoContent();
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
        [HttpGet("mi-perfil")]
        public async Task<ActionResult<UsuarioDto>> MiPerfil()
        {
            var usuarioIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (usuarioIdClaim == null)
                return Unauthorized();

            if (!int.TryParse(usuarioIdClaim.Value, out int usuarioId))
                return Unauthorized();

            var usuario = await _usuarioService
                .ObtenerPorIdAsync(usuarioId);

            if (usuario == null)
                return NotFound();

            var resultado = new UsuarioDto
            {
                Id = usuario.Id,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Email = usuario.Email,
                Estado = usuario.Estado,
                Rol = usuario.Rol.ToString()

            };

            return Ok(resultado);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id:int}/rol")]
        public async Task<IActionResult> CambiarRol(
            int id,
            CambiarRolUsuarioDto dto)
        {
            try
            {
                // Obtener el Id del administrador autenticado
                var usuarioIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier);

                if (usuarioIdClaim == null)
                    return Unauthorized();

                if (!int.TryParse(
                    usuarioIdClaim.Value,
                    out int usuarioAutenticadoId))
                {
                    return Unauthorized();
                }

                // Impedir que el administrador cambie su propio rol
                if (usuarioAutenticadoId == id)
                {
                    return BadRequest(new
                    {
                        mensaje =
                            "No puedes cambiar tu propio rol."
                    });
                }

                var usuario = await _usuarioService
                    .CambiarRolAsync(id, dto.Rol);

                return Ok(new
                {
                    usuario.Id,
                    usuario.Nombres,
                    usuario.Apellidos,
                    usuario.Email,
                    Rol = usuario.Rol.ToString()
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

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id:int}/desactivar")]
        public async Task<IActionResult> DesactivarUsuario(int id)
        {
            try
            {
                var usuarioIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier);

                if (usuarioIdClaim == null)
                    return Unauthorized();

                if (!int.TryParse(
                    usuarioIdClaim.Value,
                    out int administradorId))
                {
                    return Unauthorized();
                }

                // Evitar que el administrador se desactive a sí mismo
                if (administradorId == id)
                {
                    return BadRequest(new
                    {
                        mensaje =
                            "No puedes desactivar tu propio usuario."
                    });
                }

                var usuario = await _usuarioService
                    .DesactivarAsync(id);

                return Ok(new
                {
                    usuario.Id,
                    usuario.Nombres,
                    usuario.Apellidos,
                    usuario.Email,
                    usuario.Estado,
                    Rol = usuario.Rol.ToString()
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

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id:int}/activar")]
        public async Task<IActionResult> ActivarUsuario(int id)
        {
            try
            {
                var usuario = await _usuarioService
                    .ActivarAsync(id);

                return Ok(new
                {
                    usuario.Id,
                    usuario.Nombres,
                    usuario.Apellidos,
                    usuario.Email,
                    usuario.Estado,
                    Rol = usuario.Rol.ToString()
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
    }
}