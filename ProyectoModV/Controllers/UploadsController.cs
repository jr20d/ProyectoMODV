using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoModV.DTO.Respuestas;
using ProyectoModV.Repository.IRepository;
using ProyectoModV.SecurityConfig;
using ProyectoModV.Uploads;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoModV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly ICargaArchivo cargaArchivo;
        private readonly IJwtGenerator jwtGenerator;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IProductoRepository productoRepository;
        private readonly IVendedorRepository vendedorRepository;

        public UploadsController(ICargaArchivo cargaArchivo, IJwtGenerator jwtGenerator, IUsuarioRepository usuarioRepository,
            IProductoRepository productoRepository, IVendedorRepository vendedorRepository)
        {
            this.cargaArchivo = cargaArchivo;
            this.jwtGenerator = jwtGenerator;
            this.usuarioRepository = usuarioRepository;
            this.productoRepository = productoRepository;
            this.vendedorRepository = vendedorRepository;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation("Subir imagen para usuario")]
        [ProducesResponseType(statusCode: 201, type: typeof(RespuestaUrl))]
        [ProducesResponseType(statusCode: 409, type: typeof(RespuestaBase))]
        [ProducesResponseType(statusCode: 400, type: typeof(RespuestaBase))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CargarArchivoUsuario(IFormFile archivo)
        {
            var idUsuarioSesion = jwtGenerator.GetIdUsuarioActual();

            if (archivo.Length == 0)
            {
                return BadRequest(new RespuestaBase { Mensaje = "Ningún archivo adjuntado" });
            }
            if (archivo.ContentType.Trim() != "image/png" && archivo.ContentType.Trim() != "image/jpeg")
            {
                return BadRequest(new RespuestaBase { Mensaje = "El formato del archivo debe ser .jpg/.png" });
            }

            var usuario = await usuarioRepository.GetUsuario(idUsuarioSesion);
            string[] nombreArray = string.IsNullOrEmpty(usuario.Imagen) ? new string[] { "" } : usuario.Imagen.Split("/");
            string nombre = nombreArray.Length == 1 ? "" : nombreArray[nombreArray.Length - 1];
            var resultado = cargaArchivo.Subir(archivo, nombre);
            if (resultado.Length == 0)
            {
                Conflict(new RespuestaBase { Mensaje = "No se pudo cargar el archivo en el servidor" });
            }
            usuario.Imagen = resultado[0];
            usuario.PublicId = resultado[1];
            await usuarioRepository.UpdateUsuario(usuario);
            return Created(resultado[0], new RespuestaUrl { Url = resultado[0] });
        }

        [HttpPost("{id}")]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Subir imagen para productos por ID del producto (solo vendedores)")]
        [ProducesResponseType(statusCode: 201, type: typeof(RespuestaUrl))]
        [ProducesResponseType(statusCode: 409, type: typeof(RespuestaBase))]
        [ProducesResponseType(statusCode: 404, type: typeof(RespuestaBase))]
        [ProducesResponseType(statusCode: 400, type: typeof(RespuestaBase))]
        [ProducesResponseType(403)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CargarArchivoProductos(IFormFile archivo, int id)
        {
            var idUsuarioSesion = jwtGenerator.GetIdUsuarioActual();
            if (archivo.Length == 0)
            {
                return BadRequest(new RespuestaBase { Mensaje = "Ningún archivo adjuntado" });
            }
            if (archivo.ContentType.Trim() != "image/png" && archivo.ContentType.Trim() != "image/jpeg")
            {
                return BadRequest(new RespuestaBase { Mensaje = "El formato del archivo debe ser .jpg/.png" });
            }

            var vendedor = await vendedorRepository.GetVendedor(idUsuarioSesion);

            if (!await productoRepository.ExistsProducto(id, vendedor.VendedorId))
            {
                return NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
            }

            var producto = await productoRepository.GetProducto(id);
            string[] nombreArray = string.IsNullOrEmpty(producto.Imagen) ? new string[] { "" } : producto.Imagen.Split("/");
            string nombre = nombreArray.Length == 1 ? "" : nombreArray[nombreArray.Length - 1];
            var resultado = cargaArchivo.Subir(archivo, nombre);
            if (resultado.Length == 0)
            {
                Conflict(new RespuestaBase { Mensaje = "No se pudo cargar el archivo en el servidor" });
            }
            producto.Imagen = resultado[0];
            producto.PublicId = resultado[1];
            await productoRepository.UpdateProducto(producto);
            return Created(resultado[0], new RespuestaUrl { Url = resultado[0] });
        }
    }
}