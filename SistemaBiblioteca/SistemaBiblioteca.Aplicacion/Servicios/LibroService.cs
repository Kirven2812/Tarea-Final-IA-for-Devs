using SistemaBiblioteca.Aplicacion.Interfaces;
using SistemaBiblioteca.Aplicacion.Modelos;
using SistemaBiblioteca.Dominio.Entidades;
using SistemaBiblioteca.Dominio.Enums;

namespace SistemaBiblioteca.Aplicacion.Servicios
{
    public class LibroService
    {
        private readonly ILibroRepository _libroRepository;
        private readonly IAutorRepository _autorRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public LibroService(
            ILibroRepository libroRepository,
            IAutorRepository autorRepository,
            ICategoriaRepository categoriaRepository)
        {
            _libroRepository = libroRepository;
            _autorRepository = autorRepository;
            _categoriaRepository = categoriaRepository;
        }

        public async Task<Libro?> ObtenerPorIdAsync(int id)
        {
            return await _libroRepository.ObtenerPorIdAsync(id);
        }

        public async Task<Libro?> ObtenerPorIsbnAsync(string isbn)
        {
            return await _libroRepository.ObtenerPorIsbnAsync(isbn);
        }

        public async Task<List<Libro>> ObtenerTodosAsync()
        {
            return await _libroRepository.ObtenerTodosAsync();
        }

        public async Task<Libro> CrearAsync(
            string isbn,
            string titulo,
            string autorNombre,
            string nacionalidad,
            string categoriaNombre,
            int añoPublicacion,
            string editorial,
            string lenguaje)
        {
            // Buscar autor existente
            var autor = await _autorRepository
                .ObtenerPorNombreAsync(autorNombre);

            // Si no existe, crear uno nuevo
            if (autor == null)
            {
                autor = new Autor(
                    autorNombre,
                    nacionalidad);

                await _autorRepository
                    .AgregarAsync(autor);
            }

            // Buscar categoría existente
            var categoria = await _categoriaRepository
                .ObtenerPorNombreAsync(categoriaNombre);

            // Si no existe, crear una nueva
            if (categoria == null)
            {
                categoria = new Categoria(
                    categoriaNombre);

                await _categoriaRepository
                    .AgregarAsync(categoria);
            }

            var libro = new Libro(
                isbn,
                titulo,
                autor,
                categoria,
                añoPublicacion,
                editorial,
                lenguaje);

            await _libroRepository
                .AgregarAsync(libro);

            await _libroRepository
                .GuardarCambiosAsync();

            return libro;
        }

        public async Task<bool> ActualizarAsync(
            string isbn,
            string titulo,
            string categoriaNombre,
            string editorial,
            string lenguaje)
        {
            // 1. Buscar el libro
            var libro = await _libroRepository
                .ObtenerPorIsbnAsync(isbn);

            if (libro == null)
                return false;

            // 2. Buscar si la categoría ya existe
            var categoria = await _categoriaRepository
                .ObtenerPorNombreAsync(categoriaNombre);

            // 3. Si no existe, crearla
            if (categoria == null)
            {
                categoria = new Categoria(categoriaNombre);

                await _categoriaRepository
                    .AgregarAsync(categoria);
            }

            // 4. Actualizar la entidad Libro
            libro.Actualizar(
                titulo,
                categoria,
                editorial,
                lenguaje);

            // 5. Guardar todos los cambios
            await _libroRepository
                .GuardarCambiosAsync();

            return true;
        }

        public async Task<bool> EliminarAsync(string isbn)
        {
            var libro = await _libroRepository.ObtenerPorIsbnAsync(isbn);

            if (libro == null)
                return false;

            _libroRepository.Eliminar(libro);

            await _libroRepository.GuardarCambiosAsync();

            return true;
        }
        public async Task<ResultadoPaginado<Libro>> BuscarAsync(
            string? titulo,
            string? autor,
            string? categoria,
            EstadoLibro? estado,
            int pagina,
            int tamañoPagina)
        {
            if (pagina < 1)
                pagina = 1;

            if (tamañoPagina < 1)
                tamañoPagina = 10;

            if (tamañoPagina > 100)
                tamañoPagina = 100;

            return await _libroRepository.BuscarAsync(
                titulo,
                autor,
                categoria,
                estado,
                pagina,
                tamañoPagina);
        }
    }
}