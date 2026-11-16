
using System.ComponentModel.DataAnnotations;

namespace SistemaBiblioteca.Web.Models
{
    public class ActualizarLibroModel
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
