using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SistemaBiblioteca.Dominio.Entidades
{
    public class Autor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public string NombreAutor { get; private set; }
        public string Nacionalidad { get; private set; }

        // Constructor vacío requerido por EF
        public Autor() { }

        // Constructor parametrizado para tu lógica
        public Autor(string nombreAutor, string nacionalidad)
        {
            if (string.IsNullOrWhiteSpace(nombreAutor))
                throw new ArgumentException("El nombre del autor es obligatorio.", nameof(nombreAutor));

            if (string.IsNullOrWhiteSpace(nacionalidad))
                throw new ArgumentException("La nacionalidad es obligatoria.", nameof(nacionalidad));

            NombreAutor = nombreAutor;
            Nacionalidad = nacionalidad;
        }
    }
}
