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
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IMapper mapper;
        private readonly IJwtGenerator jwtGenerator;

        public UsuariosController(IUsuarioRepository usuarioRepository, IMapper mapper, IJwtGenerator jwtGenerator)
        {
            this.usuarioRepository = usuarioRepository;
            this.mapper = mapper;
            this.jwtGenerator = jwtGenerator;
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation("Obtener listado de usuarios")]
        [ProducesResponseType(200, Type = typeof(RespuestaListado<UsuarioDto>))]
        [ProducesResponseType(400, Type = typeof(RespuestaBase))]
        public async Task<IActionResult> GetUsuarios([FromQuery(Name = "rol")] string rol)
        {
            if (rol != "cliente" && rol != "vendedor")
            {
                return BadRequest(new RespuestaBase { Mensaje = "Debe especificar el rol 'cliente/vendedor'" });
            }
            var usuarios = await usuarioRepository.GetUsuarios(rol);
            return Ok(new RespuestaListado<UsuarioDto>
            {
                Mensaje = "Registros encontrados",
                Cantidad = usuarios.Count,
                Registros = mapper.Map<List<UsuarioDto>>(usuarios).OrderBy(u => u.Nombre).ToList()
            });
        }

        [HttpGet("registro")]
        [Authorize]
        [SwaggerOperation("Obtener usuario por ID enviado desde el JWT")]
        [ProducesResponseType(200, Type = typeof(UsuarioDetalleDto))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        public async Task<IActionResult> GetUsuario()
        {
            var id = jwtGenerator.GetIdUsuarioActual();
            var usuario = await usuarioRepository.GetUsuario(id);
            if (usuario == null)
            {
                return NotFound(new RespuestaBase { Mensaje = "El usuario no existe" });
            }
            var usuarioDetalle = mapper.Map<UsuarioDetalleDto>(usuario);
            usuarioDetalle.Roles = await usuarioRepository.GetRoles(id);
            return Ok(usuarioDetalle);
        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation("Crear un usuario")]
        [ProducesResponseType(201, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUsuario([FromBody()] CrearUsuarioDto usuarioDto)
        {
            bool[] resultados = await usuarioRepository.ExistsUsuario(usuarioDto.Correo, usuarioDto.Telefono);
            var respuesta = Validacion(resultados);

            if (respuesta != null)
            {
                return BadRequest(respuesta);
            }

            var usuario = mapper.Map<Usuario>(usuarioDto);

            return await usuarioRepository.CreateUsuario(usuario, usuarioDto.Password, "cliente") ? 
                Created($"/api/usuarios/{usuario.Id}", new RespuestaBase { Mensaje = $"El usuario {usuario.Nombre} ha sido creado" }) :
                Conflict(new RespuestaBase { Mensaje = $"No se pudo crear el usuario, inténtelo mas tarde" });
        }

        [HttpPut]
        [Authorize]
        [SwaggerOperation("Actualizar datos de usuario por ID obtenido desde el JWT")]
        [ProducesResponseType(200, Type = typeof(LoggedUsuarioDto))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        [ProducesResponseType(403)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateUsuario([FromBody()] ActualizarUsuarioDto usuarioDto)
        {
            var id = jwtGenerator.GetIdUsuarioActual();
            if (!await usuarioRepository.ExistsUsuario(id))
            {
                return NotFound(new RespuestaBase { Mensaje = "El usuario no existe" });
            }
            if (id == "")
            {
                return Forbid();
            }

            bool[] resultados = await usuarioRepository.ExistsUsuario(id, usuarioDto.Correo, usuarioDto.Telefono);
            var respuesta = Validacion(resultados);

            if (respuesta != null)
            {
                return BadRequest(respuesta);
            }

            var usuario = mapper.Map<Usuario>(usuarioDto);
            usuario.Id = id;
            var usuarioLogged = mapper.Map<LoggedUsuarioDto>(usuario);
            usuarioLogged.Token = jwtGenerator.Generar(usuario, await usuarioRepository.GetRoles(usuario.Id));
            return await usuarioRepository.UpdateUsuario(usuario) ?
                Ok(usuarioLogged) :
                Conflict(new RespuestaBase { Mensaje = $"No se pudo actualizar los datos del usuario {usuario.Nombre}, inténtelo mas tarde" });
        }

        [HttpDelete()]
        [Authorize(Roles = "cliente")]
        [SwaggerOperation("Eliminar usuario por ID obtenido desde el JWT (solo clientes)")]
        [ProducesResponseType(200, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> DeleteUsuario()
        {
            var idUsuarioSesion = jwtGenerator.GetIdUsuarioActual();
            if (idUsuarioSesion == "")
            {
                return Forbid();
            }
            if (!await usuarioRepository.ExistsUsuario(idUsuarioSesion))
            {
                return NotFound(new RespuestaBase { Mensaje = "El usuario no existe" });
            }

            return await usuarioRepository.DeleteUsuario(idUsuarioSesion, "cliente") ?
                Ok(new RespuestaBase { Mensaje = $"El usuario ha sido eliminado" }) :
                Conflict(new RespuestaBase { Mensaje = $"El usuario no pudo ser eliminado" });
        }

        [HttpPut("update-password")]
        [Authorize]
        [SwaggerOperation("Actualizar contraseña por ID de usuario obtenido desde el JWT")]
        [ProducesResponseType(200, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdatePassword([FromBody()] ActualizarPasswordDto dataPasswordDto)
        {
            var idUsuarioSesion = jwtGenerator.GetIdUsuarioActual();
            if (idUsuarioSesion == "")
            {
                return Forbid();
            }
            if (dataPasswordDto.Password != dataPasswordDto.Confirmacion)
            {
                return BadRequest(new RespuestaBase { Mensaje = "La nueva contraseña y la confirmación no coinciden" });
            }
            var usuario = await usuarioRepository.GetUsuario(idUsuarioSesion);

            if (!await usuarioRepository.ValidatePassword(usuario, dataPasswordDto.PasswordAnterior))
            {
                return BadRequest(new RespuestaBase { Mensaje = "La contraseña anterior no es válida" });
            }

            return await usuarioRepository.UpdatePassword(usuario, dataPasswordDto.Password) ?
                Ok(new RespuestaBase { Mensaje = "La contraseña ha sido actualizada" }) :
                Conflict(new RespuestaBase { Mensaje = "La contraseña no pudo ser actualizada" });
        }

        private object Validacion(bool[] resultados)
        {
            string mensajeNombre = "Ya existe un usuario con ese correo";
            string mensajeTelefono = "Ya existe un usuario con ese teléfono";

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