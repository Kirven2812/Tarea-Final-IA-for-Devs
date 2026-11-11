using Microsoft.IdentityModel.Tokens;
using SistemaBiblioteca.Aplicacion.Interfaces;
using SistemaBiblioteca.Dominio.Entidades;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistemaBiblioteca.API.Servicios
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerarToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    $"{usuario.Nombres} {usuario.Apellidos}"),

                new Claim(
                    ClaimTypes.Email,
                    usuario.Email),

                new Claim(
                    ClaimTypes.Role, 
                    usuario.Rol.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credenciales = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var expiracionMinutos =
                int.Parse(
                    _configuration["Jwt:ExpirationMinutes"]!);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    expiracionMinutos),
                signingCredentials: credenciales
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}