using System.ComponentModel.DataAnnotations;

namespace SistemaBiblioteca.Web.Models
{
    public class CrearLibroModel
    {
        [Required(ErrorMessage = "El ISBN es obligatorio.")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "El ISBN debe tener entre 10 y 13 caracteres.")]
        public string Isbn { get; set; } = string.Empty;

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del autor no puede superar los 100 caracteres.")]
        public string AutorNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(100, ErrorMessage = "La categoría no puede superar los 100 caracteres.")]
        public string CategoriaNombre { get; set; } = string.Empty;

        [Range(1500, 2100, ErrorMessage = "El año de publicación debe estar entre 1500 y 2100.")]
        public int AñoPublicacion { get; set; } = DateTime.Now.Year;

        [Required(ErrorMessage = "La editorial es obligatoria.")]
        [StringLength(100, ErrorMessage = "La editorial no puede superar los 100 caracteres.")]
        public string Editorial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El lenguaje es obligatorio.")]
        [StringLength(50, ErrorMessage = "El lenguaje no puede superar los 50 caracteres.")]
        public string Lenguaje { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nacionalidad del autor es obligatoria.")]
        [StringLength(50, ErrorMessage = "La nacionalidad no puede superar los 50 caracteres.")]
        public string Nacionalidad { get; set; } = string.Empty;
    }

    public class ActualizarLibroModel
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(100, ErrorMessage = "La categoría no puede superar los 100 caracteres.")]
        public string CategoriaNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La editorial es obligatoria.")]
        [StringLength(100, ErrorMessage = "La editorial no puede superar los 100 caracteres.")]
        public string Editorial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El lenguaje es obligatorio.")]
        [StringLength(50, ErrorMessage = "El lenguaje no puede superar los 50 caracteres.")]
        public string Lenguaje { get; set; } = string.Empty;
    }
}
