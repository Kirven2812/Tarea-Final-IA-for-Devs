using Microsoft.AspNetCore.Identity;
using SistemaBiblioteca.Aplicacion.Interfaces;
using SistemaBiblioteca.Dominio.Entidades;

namespace SistemaBiblioteca.Infraestructura.Seguridad
{
    public class PasswordHashService : IPasswordHashService
    {
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public PasswordHashService()
        {
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    "El password es obligatorio.",
                    nameof(password));
            }

            var usuario = default(Usuario);

            return _passwordHasher.HashPassword(
                usuario!,
                password);
        }

        public bool Verificar(
            string passwordHash,
            string password)
        {
            var usuario = default(Usuario);

            var resultado =
                _passwordHasher.VerifyHashedPassword(
                    usuario!,
                    passwordHash,
                    password);

            return resultado != PasswordVerificationResult.Failed;
        }
    }
}