using SistemaBiblioteca.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SistemaBiblioteca.Dominio.Entidades
{
    public class Libro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        [Required]
        public string Isbn { get; private set; }
        [Required]
        [MaxLength(200)]
        public string Titulo { get; private set; }
        public Autor Autor { get; private set; }
        public Categoria Categoria { get; private set; }
        public EstadoLibro Estado { get; private set; }
        public int AñoPublicacion { get; private set; }
        public string Editorial { get; private set; }
        public string Lenguaje { get; private set; }

        // Constructor vacío requerido por EF
        public Libro() { }

        // Constructor parametrizado
        public Libro(string isbn, string titulo, Autor autor, Categoria categoria,
                     int añoPublicacion, string editorial, string lenguaje)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentException("El ISBN es obligatorio.", nameof(isbn));

            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("El título es obligatorio.", nameof(titulo));

            if (autor == null)
                throw new ArgumentNullException(nameof(autor));

            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria));

            if (añoPublicacion <= 1450 || añoPublicacion > DateTime.Now.Year)
                throw new ArgumentException("El año no es válido!", nameof(añoPublicacion));

            if (string.IsNullOrWhiteSpace(editorial))
                throw new ArgumentException("La editorial es obligatoria.", nameof(editorial));

            if (string.IsNullOrWhiteSpace(lenguaje))
                throw new ArgumentException("El lenguaje es obligatorio.", nameof(lenguaje));

            Isbn = isbn;
            Titulo = titulo;
            Autor = autor;
            Categoria = categoria;
            Estado = EstadoLibro.Disponible;
            AñoPublicacion = añoPublicacion;
            Editorial = editorial;
            Lenguaje = lenguaje;
        }

        public void Prestar()
        {
            if (Estado == EstadoLibro.Prestado)
            {
                throw new InvalidOperationException("El libro ya se encuentra prestado!");
            }
            Estado = EstadoLibro.Prestado;
        }

        public void Devolver()
        {
            if (Estado == EstadoLibro.Disponible)
            {
                throw new InvalidOperationException("Este libro ya se encuentra disponible.!");
            }
            Estado = EstadoLibro.Disponible;
        }

        public void Actualizar(string nuevoTitulo, Categoria nuevaCategoria, string nuevaEditorial, string nuevoLenguaje)
        {
            if (string.IsNullOrWhiteSpace(nuevoTitulo))
                throw new ArgumentException("El título es obligatorio.", nameof(nuevoTitulo));

            if (nuevaCategoria == null)
                throw new ArgumentNullException(nameof(nuevaCategoria));

            if (string.IsNullOrWhiteSpace(nuevaEditorial))
                throw new ArgumentException("La editorial es obligatoria.", nameof(nuevaEditorial));

            if (string.IsNullOrWhiteSpace(nuevoLenguaje))
                throw new ArgumentException("El lenguaje es obligatorio.", nameof(nuevoLenguaje));

            Titulo = nuevoTitulo;
            Categoria = nuevaCategoria;
            Editorial = nuevaEditorial;
            Lenguaje = nuevoLenguaje;
        }


    }
}

