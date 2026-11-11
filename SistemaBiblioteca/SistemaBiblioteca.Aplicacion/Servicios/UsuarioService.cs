using SistemaBiblioteca.Aplicacion.Interfaces;
using SistemaBiblioteca.Dominio.Entidades;
using SistemaBiblioteca.Dominio.Enums;

namespace SistemaBiblioteca.Aplicacion.Servicios
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHashService _passwordHashService;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IPasswordHashService passwordHashService)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHashService = passwordHashService;
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _usuarioRepository
                .ObtenerPorIdAsync(id);
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        {
            return await _usuarioRepository
                .ObtenerTodosAsync();
        }

        public async Task<Usuario> CrearAsync(
            string nombres,
            string apellidos,
            string email,
            string password)
        {
            var existente = await _usuarioRepository
                .ObtenerPorEmailAsync(email);

            if (existente != null)
            {
                throw new InvalidOperationException(
                    "Ya existe un usuario con ese email.");
            }

            var passwordHash = _passwordHashService.Hash(password);

            var usuario = new Usuario(
                nombres,
                apellidos,
                email,
                passwordHash);

            await _usuarioRepository.AgregarAsync(usuario);
            await _usuarioRepository.GuardarCambiosAsync();

            return usuario;
        }

        public async Task<Usuario?> LoginAsync(
            string email,
            string password)
        {
            var usuario = await _usuarioRepository
                .ObtenerPorEmailAsync(email);

            if (usuario == null)
                return null;

            if (!usuario.Estado)
            {
                throw new InvalidOperationException(
                    "El usuario está inactivo.");
            }

            var passwordCorrecto =
                _passwordHashService.Verificar(
                    usuario.PasswordHash,
                    password);

            //Console.WriteLine("===== DIAGNÓSTICO LOGIN =====");
            //Console.WriteLine($"Usuario Id: {usuario.Id}");
            //Console.WriteLine($"Email: {usuario.Email}");
            //Console.WriteLine($"Hash existe: {!string.IsNullOrWhiteSpace(usuario.PasswordHash)}");
            //Console.WriteLine($"Longitud hash: {usuario.PasswordHash?.Length}");
            //Console.WriteLine($"Password recibido tiene longitud: {password?.Length}");
            //Console.WriteLine($"Password correcto: {passwordCorrecto}");
            //Console.WriteLine("==============================");

            if (!passwordCorrecto)
                return null;

            return usuario;
        }
        public async Task CambiarPasswordAsync(
            int usuarioId,
            string nuevaPassword)
        {
            var usuario = await _usuarioRepository
                .ObtenerPorIdAsync(usuarioId);

            if (usuario == null)
                throw new InvalidOperationException(
                    "El usuario no existe.");

            var nuevoHash =
                _passwordHashService.Hash(nuevaPassword);

            usuario.CambiarPasswordHash(nuevoHash);

            await _usuarioRepository.GuardarCambiosAsync();
        }
        public async Task<Usuario> CambiarRolAsync(
            int usuarioId,
            RolUsuario nuevoRol)
        {
            // Validar que el rol exista en nuestro enum
            if (!Enum.IsDefined(typeof(RolUsuario), nuevoRol))
            {
                throw new InvalidOperationException(
                    "El rol especificado no es válido.");
            }

            var usuario = await _usuarioRepository
                .ObtenerPorIdAsync(usuarioId);

            if (usuario == null)
            {
                throw new InvalidOperationException(
                    "El usuario no existe.");
            }

            usuario.CambiarRol(nuevoRol);

            await _usuarioRepository.GuardarCambiosAsync();

            return usuario;
        }

        public async Task<Usuario> DesactivarAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository
                .ObtenerPorIdAsync(usuarioId);

            if (usuario == null)
            {
                throw new InvalidOperationException(
                    "El usuario no existe.");
            }

            if (!usuario.Estado)
            {
                throw new InvalidOperationException(
                    "El usuario ya se encuentra inactivo.");
            }

            usuario.Desactivar();

            await _usuarioRepository.GuardarCambiosAsync();

            return usuario;
        }

        public async Task<Usuario> ActivarAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository
                .ObtenerPorIdAsync(usuarioId);

            if (usuario == null)
            {
                throw new InvalidOperationException(
                    "El usuario no existe.");
            }

            if (usuario.Estado)
            {
                throw new InvalidOperationException(
                    "El usuario ya se encuentra activo.");
            }

            usuario.Activar();

            await _usuarioRepository.GuardarCambiosAsync();

            return usuario;
        }
    }
}