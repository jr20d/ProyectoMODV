using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoModV.DTO.Respuestas;
using ProyectoModV.DTO.Usuarios;
using ProyectoModV.Models;
using ProyectoModV.Repository.IRepository;
using ProyectoModV.SecurityConfig;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoModV.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiAutenticacion")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IMapper mapper;
        private readonly IJwtGenerator jwtGenerator;

        public AuthController(IUsuarioRepository usuarioRepository, IMapper mapper, IJwtGenerator jwtGenerator)
        {
            this.usuarioRepository = usuarioRepository;
            this.mapper = mapper;
            this.jwtGenerator = jwtGenerator;
        }

        [HttpPost("clientes/login")]
        [AllowAnonymous]
        [SwaggerOperation("Inicio de sesión para clientes")]
        [ProducesResponseType(statusCode: 200, type: typeof(LoggedUsuarioDto))]
        [ProducesResponseType(statusCode: 409, type: typeof(RespuestaBase))]
        [ProducesResponseType(statusCode: 400, type: typeof(RespuestaBase))]
        public async Task<IActionResult> LoginCliente([FromBody()] LoginDto loginDto)
        {
            return await Login(loginDto, true);
        }

        [HttpPost("vendedores/login")]
        [AllowAnonymous]
        [SwaggerOperation("Inicio de sesión para vendedores")]
        [ProducesResponseType(statusCode: 200, type: typeof(LoggedUsuarioDto))]
        [ProducesResponseType(statusCode: 409, type: typeof(RespuestaBase))]
        [ProducesResponseType(statusCode: 400, type: typeof(RespuestaBase))]
        public async Task<IActionResult> LoginVendedor([FromBody()] LoginDto loginDto)
        {
            return await Login(loginDto, false);
        }

        [HttpGet("clientes/renew")]
        [Authorize(Roles = "cliente")]
        [SwaggerOperation("Renovar JWT para clientes")]
        [ProducesResponseType(statusCode: 200, type: typeof(RenewToken))]
        [ProducesResponseType(statusCode: 403)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RenewTokenCliente()
        {
            return await RenewToken();
        }

        [HttpGet("vendedores/renew")]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Renovar JWT para vendedores")]
        [ProducesResponseType(statusCode: 200, type: typeof(RenewToken))]
        [ProducesResponseType(statusCode: 403)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RenewTokenVendedor()
        {
            return await RenewToken();
        }

        private async Task<IActionResult> Login(LoginDto loginDto, bool isCliente)
        {
            var usuario = await usuarioRepository.GetUsuarioByEmail(loginDto.Email);

            if (usuario == null)
            {
                return Conflict(new RespuestaBase { Mensaje = "Email y/o contraseña incorrectos" });
            }

            var usuarioLogged = mapper.Map<LoggedUsuarioDto>(await usuarioRepository.Login(usuario, loginDto.Password));
            if (usuarioLogged == null)
            {
                return Conflict(new RespuestaBase { Mensaje = "Email y/o contraseña incorrectos" });
            }
            var roles = await usuarioRepository.GetRoles(usuario.Id);

            if (isCliente)
            {
                if (roles.Find(rol => rol == "vendedor") != null)
                {
                    return BadRequest(new RespuestaBase { Mensaje = "Email y/o contraseña incorrectos" });
                }                
            }
            else
            {
                if (roles.Find(rol => rol == "cliente") != null)
                {
                    return BadRequest(new RespuestaBase { Mensaje = "Email y/o contraseña incorrectos" });
                }
            }
            usuarioLogged.Token = jwtGenerator.Generar(usuario, roles);
            return Ok(usuarioLogged);
        }

        private async Task<IActionResult> RenewToken()
        {
            var dataToken = jwtGenerator.GetUsuarioActual();
            return Ok(new RenewToken
            {
                Token = jwtGenerator.Generar(mapper.Map<Usuario>(dataToken), await usuarioRepository.GetRoles(dataToken.UsuarioId))
            });
        }
    }
}
