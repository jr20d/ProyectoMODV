using Microsoft.IdentityModel.Tokens;
using ProyectoModV.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProyectoModV.SecurityConfig
{
    public class JwtGenerator : IJwtGenerator
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IConfiguration configuration;
        public JwtGenerator(IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            this.contextAccessor = contextAccessor;
            this.configuration = configuration;
        }

        public DataUsuarioToken GetUsuarioActual()
        {
            return new DataUsuarioToken
            {
                UsuarioId = contextAccessor.HttpContext.User?.Claims?.FirstOrDefault(u => u.Type == "UsuarioId")?.Value,
                Email = contextAccessor.HttpContext.User?.Claims?.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value
            };
        }

        public string GetIdUsuarioActual()
        {
            return contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(u => u.Type == "UsuarioId")?.Value ?? "";
        }

        public string Generar(Usuario usuario, List<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim("UsuarioId", usuario.Id),
                new Claim(JwtRegisteredClaimNames.NameId, usuario.Email)
            };

            roles.ForEach(rol =>
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            });

            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Secret").Value));
            var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha512Signature);

            return new JwtSecurityTokenHandler()
                .WriteToken(new JwtSecurityTokenHandler()
                .CreateToken(new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = credenciales
                }));
        }
    }
}
