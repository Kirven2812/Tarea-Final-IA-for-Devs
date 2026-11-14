using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using SistemaBiblioteca.Web.Authentication;
using SistemaBiblioteca.Web.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SistemaBiblioteca.Web.Servicios
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly CustomAuthenticationStateProvider _authenticationStateProvider;

        public UsuarioService(
            HttpClient httpClient,
            ILocalStorageService localStorage,
            NavigationManager navigationManager,
            CustomAuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> ValidarSesionActivaAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token) || TokenHaExpirado(token))
            {
                await CerrarSesionYRedirigirAsync();
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return true;
        }

        private async Task CerrarSesionYRedirigirAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _authenticationStateProvider.NotificarCerrarSesion();
            _navigationManager.NavigateTo("/login", forceLoad: false);
        }

        private async Task<bool> VerificarSesionExpiradaAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                await CerrarSesionYRedirigirAsync();
                return true;
            }

            return false;
        }

        private static bool TokenHaExpirado(string jwt)
        {
            try
            {
                var partes = jwt.Split('.');
                if (partes.Length < 2)
                    return true;

                var payload = partes[1]
                    .Replace('-', '+')
                    .Replace('_', '/');

                switch (payload.Length % 4)
                {
                    case 2:
                        payload += "==";
                        break;
                    case 3:
                        payload += "=";
                        break;
                }

                var jsonBytes = Convert.FromBase64String(payload);
                using var doc = JsonDocument.Parse(jsonBytes);

                if (!doc.RootElement.TryGetProperty("exp", out var expProp))
                    return true;

                long expSeconds = expProp.ValueKind == JsonValueKind.Number
                    ? expProp.GetInt64()
                    : long.Parse(expProp.GetString()!);

                var expDate = DateTimeOffset.FromUnixTimeSeconds(expSeconds);
                return expDate <= DateTimeOffset.UtcNow;
            }
            catch
            {
                return true;
            }
        }

        public async Task<List<UsuarioModel>> ObtenerTodosAsync()
        {
            if (!await ValidarSesionActivaAsync())
                return new List<UsuarioModel>();

            var response = await _httpClient.GetAsync("api/Usuarios");

            if (await VerificarSesionExpiradaAsync(response))
                return new List<UsuarioModel>();

            response.EnsureSuccessStatusCode();

            var resultado = await response.Content
                .ReadFromJsonAsync<List<UsuarioModel>>();

            return resultado ?? new List<UsuarioModel>();
        }

        public async Task<UsuarioModel?> ObtenerPorIdAsync(int id)
        {
            if (!await ValidarSesionActivaAsync())
                return null;

            var response = await _httpClient.GetAsync($"api/Usuarios/{id}");

            if (await VerificarSesionExpiradaAsync(response) || !response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<UsuarioModel>();
        }

        public async Task<UsuarioModel?> ObtenerMiPerfilAsync()
        {
            if (!await ValidarSesionActivaAsync())
                return null;

            var response = await _httpClient.GetAsync("api/Usuarios/mi-perfil");

            if (await VerificarSesionExpiradaAsync(response) || !response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<UsuarioModel>();
        }

        public async Task<(UsuarioModel? Usuario, string? Error)> CrearAsync(
            CrearUsuarioModel model)
        {
            if (!await ValidarSesionActivaAsync())
                return (null, "Tu sesión ha expirado. Inicia sesión nuevamente.");

            var response = await _httpClient
                .PostAsJsonAsync("api/Usuarios", model);

            if (await VerificarSesionExpiradaAsync(response))
                return (null, "Tu sesión ha expirado. Inicia sesión nuevamente.");

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
            if (!await ValidarSesionActivaAsync())
                return (false, "Tu sesión ha expirado. Inicia sesión nuevamente.");

            var dto = new CambiarRolModel { Rol = nuevoRol };
            var response = await _httpClient
                .PutAsJsonAsync($"api/Usuarios/{id}/rol", dto);

            if (await VerificarSesionExpiradaAsync(response))
                return (false, "Tu sesión ha expirado. Inicia sesión nuevamente.");

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await ExtraerMensajeErrorAsync(response);
            return (false, error);
        }

        public async Task<(bool Exito, string? Error)> CambiarPasswordAsync(
            int id,
            string nuevaPassword)
        {
            if (!await ValidarSesionActivaAsync())
                return (false, "Tu sesión ha expirado. Inicia sesión nuevamente.");

            var dto = new CambiarPasswordModel { NuevaPassword = nuevaPassword };
            var response = await _httpClient
                .PutAsJsonAsync($"api/Usuarios/{id}/password", dto);

            if (await VerificarSesionExpiradaAsync(response))
                return (false, "Tu sesión ha expirado. Inicia sesión nuevamente.");

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await ExtraerMensajeErrorAsync(response);
            return (false, error);
        }

        public async Task<(bool Exito, string? Error)> ActivarAsync(int id)
        {
            if (!await ValidarSesionActivaAsync())
                return (false, "Tu sesión ha expirado. Inicia sesión nuevamente.");

            var response = await _httpClient
                .PutAsync($"api/Usuarios/{id}/activar", null);

            if (await VerificarSesionExpiradaAsync(response))
                return (false, "Tu sesión ha expirado. Inicia sesión nuevamente.");

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await ExtraerMensajeErrorAsync(response);
            return (false, error);
        }

        public async Task<(bool Exito, string? Error)> DesactivarAsync(int id)
        {
            if (!await ValidarSesionActivaAsync())
                return (false, "Tu sesión ha expirado. Inicia sesión nuevamente.");

            var response = await _httpClient
                .PutAsync($"api/Usuarios/{id}/desactivar", null);

            if (await VerificarSesionExpiradaAsync(response))
                return (false, "Tu sesión ha expirado. Inicia sesión nuevamente.");

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
