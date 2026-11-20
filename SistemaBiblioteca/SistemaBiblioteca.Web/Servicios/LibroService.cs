using SistemaBiblioteca.Web.Models;
using SistemaBiblioteca.Web.Servicios;
using System.Net.Http.Json;
using System.Text.Json;

namespace SistemaBiblioteca.Web.Services
{
    public class LibroService
    {
        private readonly HttpClient _httpClient;
        private readonly UsuarioService _usuarioService;

        public LibroService(
            HttpClient httpClient,
            UsuarioService usuarioService)
        {
            _httpClient = httpClient;
            _usuarioService = usuarioService;
        }

        public async Task<ResultadoPaginadoModel<LibroModel>?>
            BuscarAsync(
                string? titulo = null,
                string? autor = null,
                string? categoria = null,
                string? estado = null,
                int pagina = 1,
                int tamañoPagina = 10)
        {
            if (!await _usuarioService.ValidarSesionActivaAsync())
                return null;

            var url =
                $"api/Libros/buscar?" +
                $"titulo={Uri.EscapeDataString(titulo ?? string.Empty)}&" +
                $"autor={Uri.EscapeDataString(autor ?? string.Empty)}&" +
                $"categoria={Uri.EscapeDataString(categoria ?? string.Empty)}&" +
                $"estado={Uri.EscapeDataString(estado ?? string.Empty)}&" +
                $"pagina={pagina}&" +
                $"tamañoPagina={tamañoPagina}";

            return await _httpClient
                .GetFromJsonAsync<
                    ResultadoPaginadoModel<LibroModel>>(url);
        }

        public async Task<List<LibroModel>> ObtenerTodosAsync()
        {
            if (!await _usuarioService.ValidarSesionActivaAsync())
                return new List<LibroModel>();

            var response = await _httpClient.GetAsync("api/Libros");
            if (!response.IsSuccessStatusCode)
                return new List<LibroModel>();

            var resultado = await response.Content
                .ReadFromJsonAsync<List<LibroModel>>();

            return resultado ?? new List<LibroModel>();
        }

        public async Task<(LibroModel? Libro, string? Error)> CrearAsync(CrearLibroModel model)
        {
            if (!await _usuarioService.ValidarSesionActivaAsync())
                return (null, "Tu sesión ha expirado. Inicia sesión nuevamente.");

            var response = await _httpClient.PostAsJsonAsync("api/Libros", model);

            if (response.IsSuccessStatusCode)
            {
                var libro = await response.Content.ReadFromJsonAsync<LibroModel>();
                return (libro, null);
            }

            var error = await ExtraerMensajeErrorAsync(response);
            return (null, error);
        }

        public async Task<(bool Exito, string? Error)> ActualizarAsync(string isbn, ActualizarLibroModel model)
        {
            if (!await _usuarioService.ValidarSesionActivaAsync())
                return (false, "Tu sesión ha expirado. Inicia sesión nuevamente.");

            var response = await _httpClient.PutAsJsonAsync($"api/Libros/{Uri.EscapeDataString(isbn)}", model);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await ExtraerMensajeErrorAsync(response);
            return (false, error);
        }

        public async Task<(bool Exito, string? Error)> EliminarAsync(string isbn)
        {
            if (!await _usuarioService.ValidarSesionActivaAsync())
                return (false, "Tu sesión ha expirado. Inicia sesión nuevamente.");

            var response = await _httpClient.DeleteAsync($"api/Libros/{Uri.EscapeDataString(isbn)}");

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await ExtraerMensajeErrorAsync(response);
            return (false, error);
        }

        private static async Task<string> ExtraerMensajeErrorAsync(HttpResponseMessage response)
        {
            try
            {
                var contenido = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(contenido))
                {
                    using var doc = JsonDocument.Parse(contenido);
                    if (doc.RootElement.TryGetProperty("mensaje", out var prop) &&
                        prop.ValueKind == JsonValueKind.String)
                    {
                        return prop.GetString() ?? "Ocurrió un error en la operación.";
                    }
                }
            }
            catch
            {
            }

            return "No fue posible completar la operación.";
        }
    }
}
