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


        public async Task CrearAsync(CrearLibroModel libro)
        {
            var respuesta = await _httpClient.PostAsJsonAsync(
                "api/Libros", libro);

            await ValidarRespuesta(respuesta);
        }

        public async Task ActualizarAsync(
            string isbn, ActualizarLibroModel libro)
        {
            var respuesta = await _httpClient.PutAsJsonAsync(
                $"api/Libros/{Uri.EscapeDataString(isbn)}", libro);

            await ValidarRespuesta(respuesta);
        }

        public async Task EliminarAsync(string isbn)
        {
            var respuesta = await _httpClient.DeleteAsync(
                $"api/Libros/{Uri.EscapeDataString(isbn)}");

            await ValidarRespuesta(respuesta);
        }

        private static async Task ValidarRespuesta(
            HttpResponseMessage respuesta)
        {
            if (!respuesta.IsSuccessStatusCode)
            {
                var detalle = await respuesta.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Error {(int)respuesta.StatusCode}: {detalle}");
            }
        }

        public async Task<List<AutorModel>> ObtenerAutoresAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AutorModel>>(
                "api/Autores") ?? new();
        }

        public async Task<List<CategoriaModel>> ObtenerCategoriasAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<CategoriaModel>>(
                "api/Categorias") ?? new();
        }
    }
}