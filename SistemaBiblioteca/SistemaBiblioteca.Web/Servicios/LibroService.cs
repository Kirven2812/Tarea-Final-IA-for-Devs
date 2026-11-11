using SistemaBiblioteca.Web.Models;
using System.Net.Http.Json;

namespace SistemaBiblioteca.Web.Services
{
    public class LibroService
    {
        private readonly HttpClient _httpClient;

        public LibroService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
    }
}