using SistemaBiblioteca.Dominio.Entidades;

namespace SistemaBiblioteca.Aplicacion.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerarToken(Usuario usuario);
    }
}