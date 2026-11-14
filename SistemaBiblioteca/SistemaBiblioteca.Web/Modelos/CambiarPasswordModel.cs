using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SistemaBiblioteca.Web.Models
{
    public class CambiarPasswordModel
    {
        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string NuevaPassword { get; set; } = string.Empty;

        [JsonIgnore]
        [Required(ErrorMessage = "Debes confirmar la nueva contraseña.")]
        [Compare(nameof(NuevaPassword), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}
