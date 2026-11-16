using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaBiblioteca.API.DTOs;
using SistemaBiblioteca.Aplicacion.Servicios;
using SistemaBiblioteca.Dominio.Enums;

namespace SistemaBiblioteca.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibrosController : ControllerBase
    {
        private readonly LibroService _servicio;

        public LibrosController(LibroService servicio)
        {
            _servicio = servicio;
        }

        // GET: api/libros
        /// <summary>
        /// Obtiene todos los libros disponibles en la biblioteca.
        /// </summary>
        /// <remarks>
        /// Devuelve una lista de libros con sus datos principales.
        /// </remarks>
        /// <returns>Lista de objetos <see cref="LibroDto"/>.</returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LibroDto>))]
        public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibros()
        {
            var libros = await _servicio.ObtenerTodosAsync();

            // Entidad → DTO
            var result = new List<LibroDto>();
            foreach (var libro in libros)
            {
                result.Add(new LibroDto
                {
                    Isbn = libro.Isbn,
                    Titulo = libro.Titulo,
                    AutorNombre = libro.Autor.NombreAutor,
                    CategoriaNombre = libro.Categoria.NombreCategoria,
                    Editorial = libro.Editorial,
                    Lenguaje = libro.Lenguaje,
                    AñoPublicacion = libro.AñoPublicacion,
                    Nacionalidad= libro.Autor.Nacionalidad,
                    Estado = libro.Estado.ToString()
                });
            }

            return Ok(result);
        }

        // GET: api/libros/{isbn}
        /// <summary>
        /// Obtiene un libro específico por su ISBN.
        /// </summary>
        /// <param name="isbn">Código ISBN del libro.</param>
        /// <returns>Objeto <see cref="LibroDto"/> si existe, o 404 si no se encuentra.</returns>
        [Authorize]
        [HttpGet("{isbn}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LibroDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LibroDto>> GetLibro(string isbn)
        {
            var libro = await _servicio.ObtenerPorIsbnAsync(isbn);

            if (libro == null)
                return NotFound();

            var dto = new LibroDto
            {
                Isbn = libro.Isbn,
                Titulo = libro.Titulo,
                AutorNombre = libro.Autor.NombreAutor,
                CategoriaNombre = libro.Categoria.NombreCategoria,
                Editorial = libro.Editorial,
                Lenguaje = libro.Lenguaje,
                AñoPublicacion = libro.AñoPublicacion,
                Nacionalidad = libro.Autor.Nacionalidad,
                Estado = libro.Estado.ToString()
            };

            return Ok(dto);
        }

        // POST: api/libros
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LibroDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LibroDto>> CrearLibro(CrearLibroDto dto)
        {
            var libro = await _servicio.CrearAsync(
                dto.Isbn,
                dto.Titulo,
                dto.AutorNombre,
                dto.Nacionalidad,
                dto.CategoriaNombre,
                dto.AñoPublicacion,
                dto.Editorial,
                dto.Lenguaje
            );

            var result = new LibroDto
            {
                Isbn = libro.Isbn,
                Titulo = libro.Titulo,
                AutorNombre = libro.Autor.NombreAutor,
                CategoriaNombre = libro.Categoria.NombreCategoria,
                Editorial = libro.Editorial,
                Lenguaje = libro.Lenguaje,
                AñoPublicacion = libro.AñoPublicacion,
                Nacionalidad = libro.Autor.Nacionalidad,
                Estado = libro.Estado.ToString()
            };

            return CreatedAtAction(
                nameof(GetLibro),
                new { isbn = result.Isbn },
                result);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{isbn}")]
        public async Task<IActionResult> ActualizarLibro(
            string isbn,
            ActualizarLibroDto dto)
        {
            var actualizado = await _servicio.ActualizarAsync(
                isbn,
                dto.Titulo,
                dto.CategoriaNombre,
                dto.Editorial,
                dto.Lenguaje
            );

            if (!actualizado)
                return NotFound();

            return NoContent();
        }


        [Authorize(Roles = "Administrador")]
        [HttpDelete("{isbn}")]
        public async Task<IActionResult> EliminarLibro(string isbn)
        {
            var eliminado = await _servicio.EliminarAsync(isbn);

            if (!eliminado)
                return NotFound();

            return NoContent();
        }
        [Authorize]
        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar(
            [FromQuery] string? titulo,
            [FromQuery] string? autor,
            [FromQuery] string? categoria,
            [FromQuery] EstadoLibro? estado,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamañoPagina = 10)
        {
            var resultado = await _servicio.BuscarAsync(
                titulo,
                autor,
                categoria,
                estado,
                pagina,
                tamañoPagina);

            var libros = resultado.Items.Select(libro => new LibroDto
            {
                Isbn = libro.Isbn,
                Titulo = libro.Titulo,
                AutorNombre = libro.Autor.NombreAutor,
                CategoriaNombre = libro.Categoria.NombreCategoria,
                Editorial = libro.Editorial,
                Lenguaje = libro.Lenguaje,
                AñoPublicacion = libro.AñoPublicacion,
                Nacionalidad = libro.Autor.Nacionalidad,
                Estado = libro.Estado.ToString()
            });

            return Ok(new
            {
                resultado.Pagina,
                resultado.TamañoPagina,
                resultado.TotalRegistros,
                resultado.TotalPaginas,
                Items = libros
            });
        }
    }
}
