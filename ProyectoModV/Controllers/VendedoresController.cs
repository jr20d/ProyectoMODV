using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoModV.DTO.Respuestas;
using ProyectoModV.DTO.Usuarios;
using ProyectoModV.DTO.Vendedores;
using ProyectoModV.Models;
using ProyectoModV.Repository.IRepository;
using ProyectoModV.SecurityConfig;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoModV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendedoresController : ControllerBase
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IVendedorRepository vendedorRepository;
        private readonly IJwtGenerator jwtGenerator;
        private readonly IMapper mapper;
        public VendedoresController(IMapper mapper, IUsuarioRepository usuarioRepository, IVendedorRepository vendedorRepository, IJwtGenerator jwtGenerator)
        {
            this.mapper = mapper;
            this.usuarioRepository = usuarioRepository;
            this.vendedorRepository = vendedorRepository;
            this.jwtGenerator = jwtGenerator;
        }

        [HttpGet]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Obtener vendedor por ID de usuario obtenido desde el JWT (solo vendedores)")]
        [ProducesResponseType(200, Type = typeof(VendedorDto))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        public async Task<IActionResult> GetVendedor()
        {
            var id = jwtGenerator.GetIdUsuarioActual();

            if (id == "")
            {
                return Unauthorized();
            }

            if (!await usuarioRepository.ExistsUsuario(id))
            {
                return NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
            }
            var vendedor = await vendedorRepository.GetVendedor(id);
            return vendedor != null ? Ok(mapper.Map<VendedorDto>(vendedor)) : NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation("Crear un vendedor")]
        [ProducesResponseType(201, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateVendedor([FromBody] CrearUsuarioDto usuarioDto)
        {
            bool[] resultados = await usuarioRepository.ExistsUsuario(usuarioDto.Correo, usuarioDto.Telefono);
            var respuesta = Validacion(resultados);

            if (respuesta != null)
            {
                return BadRequest(respuesta);
            }

            var usuario = mapper.Map<Usuario>(usuarioDto);
            if (!await usuarioRepository.CreateUsuario(usuario, usuarioDto.Password, "vendedor"))
            {
                return Conflict(new RespuestaBase { Mensaje = "No se pudo crear el vendedor, inténtelo mas tarde" });
            }
            var vendedor = new Vendedor { UsuarioId = usuario.Id };

            return await vendedorRepository.CreateVendedor(vendedor) ?
                Created($"/api/vendedores/{vendedor.VendedorId}", new RespuestaBase { Mensaje = $"El vendedor {usuario.Nombre} ha sido creado" }) :
                Conflict(new RespuestaBase { Mensaje = $"No se pudo crear el vendedor, inténtelo mas tarde" });
        }

        [HttpDelete]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Eliminar vendedor por ID de usuario obtenido desde el JWT (solo vendedores)")]
        [ProducesResponseType(200, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> DeleteVendedor()
        {
            var idUsuarioSesion = jwtGenerator.GetIdUsuarioActual();

            if (idUsuarioSesion == "")
            {
                return Unauthorized();
            }

            var vendedor = await vendedorRepository.GetVendedor(idUsuarioSesion);

            if (vendedor == null)
            {
                return NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
            }
            await vendedorRepository.DeleteVendedor(vendedor);
            await usuarioRepository.DeleteUsuario(vendedor.Usuario.Id, "vendedor");

            return Ok(new RespuestaBase { Mensaje = $"El vendedor ha sido eliminado"});
        }

        private object Validacion(bool[] resultados)
        {
            string mensajeNombre = "Ya existe un vendedor/usuario con ese nombre";
            string mensajeTelefono = "Ya existe un vendedor/usuario con ese teléfono";

            if (resultados[0] || resultados[1])
            {
                return (resultados[0] && resultados[1] ? new
                {
                    Errores = new
                    {
                        Nombre = mensajeNombre,
                        Telefono = mensajeTelefono
                    }
                } :
                    (resultados[0] ? new
                    {
                        Errores = new
                        {
                            Nombre = mensajeNombre
                        }
                    } :
                    new
                    {
                        Errores = new
                        {
                            Telefono = mensajeTelefono
                        }
                    })
                );
            }
            return null;
        }
    }
}
