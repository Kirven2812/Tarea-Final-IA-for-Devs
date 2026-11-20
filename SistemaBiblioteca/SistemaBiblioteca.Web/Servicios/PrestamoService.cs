using SistemaBiblioteca.Web.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace SistemaBiblioteca.Web.Servicios
{
    public class PrestamoService
    {
        private readonly HttpClient _httpClient;
        private readonly UsuarioService _usuarioService;

        public PrestamoService(
            HttpClient httpClient,
            UsuarioService usuarioService)
        {
            _httpClient = httpClient;
            _usuarioService = usuarioService;
        }

        public async Task<List<PrestamoModel>> ObtenerMisPrestamosAsync()
        {
            if (!await _usuarioService.ValidarSesionActivaAsync())
                return new List<PrestamoModel>();

            var response = await _httpClient.GetAsync("api/Prestamos/mis-prestamos");

            if (!response.IsSuccessStatusCode)
                return new List<PrestamoModel>();

            var resultado = await response.Content
                .ReadFromJsonAsync<List<PrestamoModel>>();

            return resultado ?? new List<PrestamoModel>();
        }

        public async Task<(bool Exito, string? Error)> DevolverLibroAsync(int prestamoId)
        {
            if (!await _usuarioService.ValidarSesionActivaAsync())
                return (false, "Tu sesión ha expirado. Inicia sesión nuevamente.");

            var response = await _httpClient.PutAsync($"api/Prestamos/{prestamoId}/devolver", null);

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
