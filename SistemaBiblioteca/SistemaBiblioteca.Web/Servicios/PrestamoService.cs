
using Blazored.LocalStorage;
using SistemaBiblioteca.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SistemaBiblioteca.Web.Servicios
{
    public class PrestamoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public PrestamoService(
            HttpClient httpClient,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<List<PrestamoModel>>
            ObtenerMisPrestamosAsync()
        {
            var token = await _localStorage
                .GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException(
                    "No se encontró una sesión activa.");
            }

            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/prestamos/mis-prestamos");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            using var response =
                await _httpClient.SendAsync(request);

            await ValidarRespuestaAsync(response);

            return await response.Content
                .ReadFromJsonAsync<List<PrestamoModel>>()
                ?? new List<PrestamoModel>();
        }

        public async Task DevolverLibroAsync(int prestamoId)
        {
            var token = await _localStorage
                .GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException(
                    "No se encontró una sesión activa.");
            }

            using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"api/prestamos/{prestamoId}/devolver");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            using var response =
                await _httpClient.SendAsync(request);

            await ValidarRespuestaAsync(response);

        }

        public async Task CrearPrestamoAsync(int libroId)
        {
            var token = await _localStorage
                .GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException(
                    "No se encontró una sesión activa.");
            }

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/prestamos");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Content = JsonContent.Create(new
            {
                LibroId = libroId
            });

            using var response =
                await _httpClient.SendAsync(request);

            await ValidarRespuestaAsync(response);
        }

        private static async Task ValidarRespuestaAsync(
            HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;

            if (response.StatusCode ==
                System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException(
                    "Tu sesión ha expirado. Inicia sesión nuevamente.");
            }

            if (response.StatusCode ==
                System.Net.HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException(
                    "No tienes permiso para realizar esta operación.");
            }

            if (response.StatusCode ==
                System.Net.HttpStatusCode.BadRequest)
            {
                try
                {
                    using var contenido = await response.Content
                        .ReadFromJsonAsync<System.Text.Json.JsonDocument>();

                    if (contenido != null &&
                        contenido.RootElement.TryGetProperty(
                            "mensaje", out var mensaje))
                    {
                        var texto = mensaje.GetString();

                        if (!string.IsNullOrWhiteSpace(texto))
                            throw new InvalidOperationException(texto);
                    }
                }
                catch (System.Text.Json.JsonException)
                {
                    // La API devolvió una respuesta no compatible.
                }

                throw new InvalidOperationException(
                    "La operación fue rechazada por la API.");
            }

            throw new HttpRequestException(
                $"Error al comunicarse con la API. HTTP {(int)response.StatusCode}.");
        }


    }
}
