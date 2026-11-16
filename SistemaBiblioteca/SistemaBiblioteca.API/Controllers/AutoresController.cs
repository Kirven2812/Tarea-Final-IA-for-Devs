
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaBiblioteca.Aplicacion.Interfaces;

namespace SistemaBiblioteca.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AutoresController : ControllerBase
    {
        private readonly IAutorRepository _repositorio;

        public AutoresController(IAutorRepository repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<IActionResult> GetAutores()
        {
            var autores = await _repositorio.ObtenerTodosAsync();

            return Ok(autores.Select(a => new
            {
                a.NombreAutor,
                a.Nacionalidad
            }));
        }
    }
}
