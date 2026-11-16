
using System.ComponentModel.DataAnnotations;

namespace SistemaBiblioteca.Web.Models
{
    public class CrearLibroModel
    {
        [Required(ErrorMessage = "El ISBN es obligatorio")]
        [StringLength(13, MinimumLength = 10)]
        public string Isbn { get; set; } = string.Empty;

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(100)]
        public string AutorNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(100)]
        public string CategoriaNombre { get; set; } = string.Empty;

        [Range(1500, 2100)]
        public int AñoPublicacion { get; set; }

        [Required(ErrorMessage = "La editorial es obligatoria")]
        [StringLength(100)]
        public string Editorial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El lenguaje es obligatorio")]
        [StringLength(50)]
        public string Lenguaje { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nacionalidad es obligatoria")]
        [StringLength(50)]
        public string Nacionalidad { get; set; } = string.Empty;
    }
}
