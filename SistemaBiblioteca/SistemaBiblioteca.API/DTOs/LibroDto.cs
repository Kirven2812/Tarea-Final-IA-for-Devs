using System.ComponentModel.DataAnnotations;

namespace SistemaBiblioteca.API.DTOs
{
    public class LibroDto
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

    public class CrearLibroDto
    {
        [Required]
        [StringLength(13, MinimumLength = 10)]
        public string Isbn { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string AutorNombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CategoriaNombre { get; set; } = string.Empty;

        [Range(1500, 2100)]
        public int AñoPublicacion { get; set; }

        [Required]
        [StringLength(100)]
        public string Editorial { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Lenguaje { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Nacionalidad { get; set; } = string.Empty;
    }

    public class ActualizarLibroDto
    {
        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CategoriaNombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Editorial { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Lenguaje { get; set; } = string.Empty;
    }
}