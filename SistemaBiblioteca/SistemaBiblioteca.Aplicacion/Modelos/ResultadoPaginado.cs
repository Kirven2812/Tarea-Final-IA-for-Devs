namespace SistemaBiblioteca.Aplicacion.Modelos
{
    public class ResultadoPaginado<T>
    {
        public List<T> Items { get; set; } = new();

        public int Pagina { get; set; }

        public int TamañoPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas =>
            (int)Math.Ceiling(
                TotalRegistros / (double)TamañoPagina);
    }
}