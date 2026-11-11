using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.Dominio.Entidades;

namespace SistemaBiblioteca.Infraestructura.Persistencia
{
    public class BibliotecaDbContext : DbContext
    {
        public BibliotecaDbContext(DbContextOptions<BibliotecaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Libro> Libros { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Prestamo> Prestamos{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //libro - PK 
            modelBuilder.Entity<Libro>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Libro>()
                .Property(l => l.Id)
                .ValueGeneratedOnAdd(); // Identity en SQL Server

            modelBuilder.Entity<Libro>()
                .HasIndex(l => l.Isbn)
                .IsUnique();

            // Autor - PK 
            modelBuilder.Entity<Autor>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Autor>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Autor>()
                .HasIndex(a => a.NombreAutor)
                .IsUnique();

            // Categoria - PK
            modelBuilder.Entity<Categoria>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Categoria>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Categoria>()
                .HasIndex(c => c.NombreCategoria)
                .IsUnique();

            // Usuario - PK
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Rol)
                .HasConversion<int>();

            // Préstamo - PK
            modelBuilder.Entity<Prestamo>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Prestamo>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            // Relaciones entre Prestamo, Usuario y Libro
            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Prestamos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Libro)
                .WithMany()
                .HasForeignKey(p => p.LibroId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }

}
