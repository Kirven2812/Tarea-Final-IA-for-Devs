using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace SistemaBiblioteca.Web.Authentication
{
    public class CustomAuthenticationStateProvider
        : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;

        public CustomAuthenticationStateProvider(
            ILocalStorageService localStorage,
            HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState>
            GetAuthenticationStateAsync()
        {
            var token = await _localStorage
                .GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
                return UsuarioAnonimo();

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        token);

                var claims = ParseClaimsFromJwt(token);

                if (TokenExpirado(claims))
                {
                    await _localStorage
                        .RemoveItemAsync("authToken");

                    _httpClient.DefaultRequestHeaders.Authorization =
                        null;

                    return UsuarioAnonimo();
                }

                var identity = new ClaimsIdentity(
                    claims,
                    authenticationType: "jwt");

                var usuario =
                    new ClaimsPrincipal(identity);

                return new AuthenticationState(usuario);
            }
            catch
            {
                await _localStorage
                    .RemoveItemAsync("authToken");

                _httpClient.DefaultRequestHeaders.Authorization =
                    null;

                return UsuarioAnonimo();
            }
        }

        public void NotificarUsuarioAutenticado(
            string token)
        {
            var claims = ParseClaimsFromJwt(token);

            var identity = new ClaimsIdentity(
                claims,
                authenticationType: "jwt");

            var usuario =
                new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(
                Task.FromResult(
                    new AuthenticationState(usuario)));
        }

        public void NotificarCerrarSesion()
        {
            var usuario =
                new ClaimsPrincipal(
                    new ClaimsIdentity());

            NotifyAuthenticationStateChanged(
                Task.FromResult(
                    new AuthenticationState(usuario)));
        }

        private static AuthenticationState UsuarioAnonimo()
        {
            var usuario =
                new ClaimsPrincipal(
                    new ClaimsIdentity());

            return new AuthenticationState(usuario);
        }

        private static bool TokenExpirado(IEnumerable<Claim> claims)
        {
            var expirationClaim = claims
                .FirstOrDefault(c => c.Type == "exp");

            if (expirationClaim == null)
                return true;

            if (!long.TryParse(
                expirationClaim.Value,
                out var expirationSeconds))
            {
                return true;
            }

            var expirationDate =
                DateTimeOffset.FromUnixTimeSeconds(
                    expirationSeconds);

            return expirationDate <= DateTimeOffset.UtcNow;
        }

        private static IEnumerable<Claim>
            ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];

            var jsonBytes =
                ParseBase64WithoutPadding(payload);

            var keyValuePairs =
                JsonSerializer.Deserialize<
                    Dictionary<string, JsonElement>>(
                        jsonBytes);

            var claims = new List<Claim>();

            if (keyValuePairs == null)
                return claims;

            foreach (var item in keyValuePairs)
            {
                if (item.Value.ValueKind ==
                    JsonValueKind.Array)
                {
                    foreach (var elemento
                             in item.Value.EnumerateArray())
                    {
                        claims.Add(
                            new Claim(
                                item.Key,
                                elemento.ToString()));
                    }
                }
                else
                {
                    claims.Add(
                        new Claim(
                            item.Key,
                            item.Value.ToString()));
                }
            }

            return claims;
        }

        private static byte[]
            ParseBase64WithoutPadding(string base64)
        {
            base64 = base64
                .Replace('-', '+')
                .Replace('_', '/');

            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;

                case 3:
                    base64 += "=";
                    break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}