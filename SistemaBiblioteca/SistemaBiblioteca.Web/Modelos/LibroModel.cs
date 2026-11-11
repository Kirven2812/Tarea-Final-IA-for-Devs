namespace SistemaBiblioteca.Web.Models
{
    public class LibroModel
    {
        public string Isbn { get; set; } = string.Empty;

        public string Titulo { get; set; } = string.Empty;

        public string AutorNombre { get; set; } = string.Empty;

        public string CategoriaNombre { get; set; } = string.Empty;

        public int AñoPublicacion { get; set; }

        public string Editorial { get; set; } = string.Empty;

        public string Lenguaje { get; set; } = string.Empty;

        public string Nacionalidad { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;
    }
}