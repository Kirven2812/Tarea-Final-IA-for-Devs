using Blazored.LocalStorage;
using SistemaBiblioteca.Web.Authentication;
using SistemaBiblioteca.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SistemaBiblioteca.Web.Servicios
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly CustomAuthenticationStateProvider _authenticationStateProvider;

        public AuthService(
            HttpClient httpClient,
            ILocalStorageService localStorage,
            CustomAuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider =
                authenticationStateProvider;
        }

        public async Task<LoginResponse?> LoginAsync(
            string email,
            string password)
        {
            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/Usuarios/login",
                request);

            if (!response.IsSuccessStatusCode)
                return null;

            var resultado = await response.Content
                .ReadFromJsonAsync<LoginResponse>();

            if (resultado == null ||
                string.IsNullOrWhiteSpace(resultado.Token))
            {
                return null;
            }

            // Guardamos el JWT
            await _localStorage.SetItemAsync(
                "authToken",
                resultado.Token);

            // Configuramos HttpClient para las siguientes peticiones
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    resultado.Token);

            _authenticationStateProvider
                .NotificarUsuarioAutenticado(resultado.Token);

            return resultado;
        }
    }
}