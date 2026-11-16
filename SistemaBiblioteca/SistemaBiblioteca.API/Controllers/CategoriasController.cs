
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaBiblioteca.Aplicacion.Interfaces;

namespace SistemaBiblioteca.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepository _repositorio;

        public CategoriasController(ICategoriaRepository repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _repositorio.ObtenerTodosAsync();

            return Ok(categorias.Select(c => new
            {
                c.NombreCategoria
            }));
        }
    }
}
