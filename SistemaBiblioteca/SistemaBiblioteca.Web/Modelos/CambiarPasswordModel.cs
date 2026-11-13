using System.ComponentModel.DataAnnotations;

namespace SistemaBiblioteca.Web.Models
{
    public class CambiarPasswordModel
    {
        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string NuevaPassword { get; set; } = string.Empty;
    }
}
