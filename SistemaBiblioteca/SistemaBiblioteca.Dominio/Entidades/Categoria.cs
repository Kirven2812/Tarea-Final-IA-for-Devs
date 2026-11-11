using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SistemaBiblioteca.Dominio.Entidades
{
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public string NombreCategoria { get; private set; }

        // Constructor vacío requerido por EF
        public Categoria() { }

        // Constructor parametrizado
        public Categoria(string nombreCategoria)
        {
            if (string.IsNullOrWhiteSpace(nombreCategoria))
                throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(nombreCategoria));

            NombreCategoria = nombreCategoria;
        }
    }
}
