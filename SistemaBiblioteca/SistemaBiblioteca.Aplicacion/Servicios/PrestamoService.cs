using SistemaBiblioteca.Aplicacion.Interfaces;
using SistemaBiblioteca.Dominio.Entidades;

public class PrestamoService
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly ILibroRepository _libroRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public PrestamoService(
        IPrestamoRepository prestamoRepository,
        ILibroRepository libroRepository,
        IUsuarioRepository usuarioRepository)
    {
        _prestamoRepository = prestamoRepository;
        _libroRepository = libroRepository;
        _usuarioRepository = usuarioRepository;
    }
    public async Task<Prestamo> PrestarLibroAsync(
        int usuarioId,
        int libroId)
    {

        var usuario = await _usuarioRepository
            .ObtenerPorIdAsync(usuarioId);

        if (usuario == null)
            throw new InvalidOperationException(
                "El usuario no existe.");

        var libro = await _libroRepository
            .ObtenerPorIdAsync(libroId);

        // Primero verificamos que el libro exista.
        if (libro == null)
            throw new InvalidOperationException(
                "El libro no existe.");

        var prestamo = Prestamo.Crear(
            usuario,
            libro);

        await _prestamoRepository
            .AgregarAsync(prestamo);

        await _prestamoRepository
            .GuardarCambiosAsync();

        return prestamo;
    }
    public async Task<Prestamo> DevolverLibroAsync(
        int prestamoId,
        int usuarioId)
    {
        var prestamo = await _prestamoRepository
            .ObtenerPorIdAsync(prestamoId);

        if (prestamo == null)
        {
            throw new InvalidOperationException(
                "El préstamo no existe.");
        }

        if (prestamo.UsuarioId != usuarioId)
        {
            throw new UnauthorizedAccessException(
                "No puedes devolver un préstamo que pertenece a otro usuario.");
        }

        prestamo.Devolver();

        await _prestamoRepository
            .GuardarCambiosAsync();

        return prestamo;
    }

    public async Task<Prestamo?> ObtenerPorIdAsync(int id)
    {
        return await _prestamoRepository
            .ObtenerPorIdAsync(id);
    }

    public async Task<List<Prestamo>> ObtenerTodosAsync()
    {
        return await _prestamoRepository
            .ObtenerTodosAsync();
    }

    public async Task<List<Prestamo>> ObtenerPorUsuarioAsync(
        int usuarioId)
    {
        var usuario = await _usuarioRepository
            .ObtenerPorIdAsync(usuarioId);

        if (usuario == null)
        {
            throw new InvalidOperationException(
                "El usuario no existe.");
        }

        return await _prestamoRepository
            .ObtenerPorUsuarioAsync(usuarioId);
    }

}