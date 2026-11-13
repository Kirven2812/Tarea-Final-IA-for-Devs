using Blazored.LocalStorage;
using SistemaBiblioteca.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SistemaBiblioteca.Web.Servicios
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public UsuarioService(
            HttpClient httpClient,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        private async Task AsegurarTokenAsync()
        {
            if (_httpClient.DefaultRequestHeaders.Authorization == null)
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
            }
        }

        public async Task<List<UsuarioModel>> ObtenerTodosAsync()
        {
            await AsegurarTokenAsync();

            var resultado = await _httpClient
                .GetFromJsonAsync<List<UsuarioModel>>("api/Usuarios");

            return resultado ?? new List<UsuarioModel>();
        }

        public async Task<UsuarioModel?> ObtenerPorIdAsync(int id)
        {
            await AsegurarTokenAsync();

            return await _httpClient
                .GetFromJsonAsync<UsuarioModel>($"api/Usuarios/{id}");
        }

        public async Task<UsuarioModel?> ObtenerMiPerfilAsync()
        {
            await AsegurarTokenAsync();

            return await _httpClient
                .GetFromJsonAsync<UsuarioModel>("api/Usuarios/mi-perfil");
        }

        public async Task<(UsuarioModel? Usuario, string? Error)> CrearAsync(
            CrearUsuarioModel model)
        {
            await AsegurarTokenAsync();

            var response = await _httpClient
                .PostAsJsonAsync("api/Usuarios", model);

            if (response.IsSuccessStatusCode)
            {
                var usuario = await response.Content
                    .ReadFromJsonAsync<UsuarioModel>();
                return (usuario, null);
            }

            var error = await ExtraerMensajeErrorAsync(response);
            return (null, error);
        }

        public async Task<(bool Exito, string? Error)> CambiarRolAsync(
            int id,
            int nuevoRol)
        {
            await AsegurarTokenAsync();

            var dto = new CambiarRolModel { Rol = nuevoRol };
            var response = await _httpClient
                .PutAsJsonAsync($"api/Usuarios/{id}/rol", dto);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await ExtraerMensajeErrorAsync(response);
            return (false, error);
        }

        public async Task<(bool Exito, string? Error)> CambiarPasswordAsync(
            int id,
            string nuevaPassword)
        {
            await AsegurarTokenAsync();

            var dto = new CambiarPasswordModel { NuevaPassword = nuevaPassword };
            var response = await _httpClient
                .PutAsJsonAsync($"api/Usuarios/{id}/password", dto);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await ExtraerMensajeErrorAsync(response);
            return (false, error);
        }

        public async Task<(bool Exito, string? Error)> ActivarAsync(int id)
        {
            await AsegurarTokenAsync();

            var response = await _httpClient
                .PutAsync($"api/Usuarios/{id}/activar", null);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await ExtraerMensajeErrorAsync(response);
            return (false, error);
        }

        public async Task<(bool Exito, string? Error)> DesactivarAsync(int id)
        {
            await AsegurarTokenAsync();

            var response = await _httpClient
                .PutAsync($"api/Usuarios/{id}/desactivar", null);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await ExtraerMensajeErrorAsync(response);
            return (false, error);
        }

        private static async Task<string> ExtraerMensajeErrorAsync(
            HttpResponseMessage response)
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
                // Ignorar error de parseo y devolver mensaje genérico
            }

            return "No fue posible completar la operación.";
        }
    }
}
