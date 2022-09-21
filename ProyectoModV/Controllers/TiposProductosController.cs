using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoModV.DTO.Respuestas;
using ProyectoModV.DTO.TiposProductos;
using ProyectoModV.Models;
using ProyectoModV.Repository.IRepository;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoModV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiTipoProducto")]
    public class TiposProductosController : ControllerBase
    {
        private readonly ITipoProductoRepository tipoProductoRepository;
        private readonly IMapper mapper;

        public TiposProductosController(ITipoProductoRepository tipoProductoRepository, IMapper mapper)
        {
            this.tipoProductoRepository = tipoProductoRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation("Obtener listado de tipos de productos")]
        [ProducesResponseType(200, Type = typeof(RespuestaListado<TipoProductoDto>))]
        public async Task<IActionResult> GetTiposProductos()
        {
            var tProductos = await tipoProductoRepository.GetTiposProductos();

            return Ok(new RespuestaListado<TipoProductoDto>
            {
                Mensaje = "Resultados",
                Cantidad = tProductos.Count,
                Registros = mapper.Map<List<TipoProductoDto>>(tProductos)
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Obtener tipo producto por ID (solo vendedores)")]
        [ProducesResponseType(200, Type = typeof(TipoProductoDto))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetTipoProducto(int id)
        {
            var tipoProducto = await tipoProductoRepository.GetTipoProducto(id);
            if (tipoProducto == null)
            {
                return NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
            }
            return Ok(mapper.Map<TipoProductoDto>(tipoProducto));
        }

        [HttpPost]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Agregar tipo de producto (solo vendedores)")]
        [ProducesResponseType(201, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(403)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTipoProducto([FromBody()] TipoProductoBaseDto tipoProductoDto)
        {
            if (await tipoProductoRepository.ExistsTipoProducto(tipoProductoDto.Tipo))
            {
                return Conflict(new RespuestaBase { Mensaje = $"El tipo de producto {tipoProductoDto.Tipo} ya existe" });
            }
            var tipoProducto = mapper.Map<TipoProducto>(tipoProductoDto);
            return await tipoProductoRepository.CreateTipoProducto(tipoProducto) ?
                Created($"/api/tiposproductos/{tipoProducto.TipoProductoId}", new RespuestaBase { Mensaje = $"El tipo de producto {tipoProducto.Tipo} ha sido creado" }):
                Conflict(new RespuestaBase { Mensaje = $"El tipo de producto {tipoProducto.Tipo} no pudo ser creado" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Actualizar tipo de producto por ID (solo vendedores)")]
        [ProducesResponseType(200, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        [ProducesResponseType(403)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateTipoProducto([FromBody()] TipoProductoBaseDto tipoProductoDto, int id)
        {
            if (!await tipoProductoRepository.ExistsTipoProducto(id))
            {
                return NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
            }
            if (await tipoProductoRepository.ExistsTipoProducto(id, tipoProductoDto.Tipo))
            {
                return Conflict(new RespuestaBase { Mensaje = $"Ya existe un registro con el tipo de producto {tipoProductoDto.Tipo}" });
            }

            var tipoProducto = mapper.Map<TipoProducto>(tipoProductoDto);
            tipoProducto.TipoProductoId = id;

            return await tipoProductoRepository.UpdateTipoProducto(tipoProducto) ?
                Ok(new RespuestaBase { Mensaje = $"Los datos del tipo de producto {tipoProducto.Tipo} han sido actualizados" }) :
                Conflict(new RespuestaBase { Mensaje = $"Los datos del tipo de producto {tipoProducto.Tipo} no fueron actualizados" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Eliminar tipo de producto por ID (solo vendedores)")]
        [ProducesResponseType(200, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> DeleteTipoProducto(int id)
        {
            if (!await tipoProductoRepository.ExistsTipoProducto(id))
            {
                return NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
            }
            var tipoProducto = await tipoProductoRepository.GetTipoProducto(id);
            if (tipoProducto.Productos.Count > 0)
            {
                return Conflict(new RespuestaBase { Mensaje = $"Hay productos relacionados al tipo de producto {tipoProducto.Tipo} y no puede ser eliminado" });
            }

            return await tipoProductoRepository.DeleteTipoProducto(tipoProducto) ?
                Ok(new RespuestaBase { Mensaje = $"El tipo de producto {tipoProducto.Tipo} ha sido eliminado" }) :
                Conflict(new RespuestaBase { Mensaje = $"El tipo de producto {tipoProducto.Tipo} no pudo ser eliminado" });
        }
    }
}