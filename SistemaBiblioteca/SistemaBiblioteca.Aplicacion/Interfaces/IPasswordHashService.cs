namespace SistemaBiblioteca.Aplicacion.Interfaces
{
    public interface IPasswordHashService
    {
        string Hash(string password);
        bool Verificar(string passwordHash, string password);
    }
}