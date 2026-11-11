namespace SistemaBiblioteca.Web.Models
{
    public class ResultadoPaginadoModel<T>
    {
        public int Pagina { get; set; }

        public int TamañoPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas { get; set; }

        public List<T> Items { get; set; } = new();
    }
}