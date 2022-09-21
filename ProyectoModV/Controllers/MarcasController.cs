using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoModV.DTO.Marcas;
using ProyectoModV.DTO.Respuestas;
using ProyectoModV.Models;
using ProyectoModV.Repository.IRepository;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoModV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiMarcas")]
    public class MarcasController : ControllerBase
    {
        private readonly IMarcaRepository marcaRepository;
        private readonly IMapper mapper;

        public MarcasController(IMarcaRepository marcaRepository, IMapper mapper)
        {
            this.marcaRepository = marcaRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation("Obtener listado de marcas")]
        [ProducesResponseType(200, Type = typeof(RespuestaListado<MarcaDto>))]
        public async Task<IActionResult> GetMarcas()
        {
            var marcas = await marcaRepository.GetMarcas();
            return Ok(new RespuestaListado<MarcaDto>
            {
                Mensaje = "Resultado",
                Cantidad = marcas.Count,
                Registros = mapper.Map<List<MarcaDto>>(marcas)
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Obtener registro de marca por ID (solo vendedores)")]
        [ProducesResponseType(200, Type = typeof(MarcaDto))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        public async Task<IActionResult> GetMarca(int id)
        {
            var marca = await marcaRepository.GetMarca(id);

            if (marca == null)
            {
                return NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
            }

            return Ok(mapper.Map<MarcaDto>(marca));
        }

        [HttpPost]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Agregar marca (solo vendedores)")]
        [ProducesResponseType(201, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateMarca([FromBody()] MarcaBaseDto marcaDto)
        {
            if (await marcaRepository.ExistsMarca(marcaDto.Nombre))
            {
                return Conflict(new RespuestaBase { Mensaje = $"Ya existe otro registro de marca con el nombre de {marcaDto.Nombre}" });
            }
            var marca = mapper.Map<Marca>(marcaDto);
            return await marcaRepository.CreateMarca(marca) ?
                Created($"/api/marcas/{marca.MarcaId}", new RespuestaBase { Mensaje = $"El registro {marca.Nombre} ha sido creado" }) :
                Conflict(new RespuestaBase { Mensaje = $"El regitro {marca.Nombre} no pudo ser creado" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Actualizar marca por ID (solo vendedores)")]
        [ProducesResponseType(200, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateMarca([FromBody()] MarcaBaseDto marcaDto, int id)
        {
            if (!await marcaRepository.ExistsMarca(id))
            {
                return NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
            }
            if (await marcaRepository.ExistsMarca(id, marcaDto.Nombre))
            {
                return Conflict(new RespuestaBase { Mensaje = $"Ya existe otro registro de marca con el nombre de {marcaDto.Nombre}" });
            }
            var marca = mapper.Map<Marca>(marcaDto);
            marca.MarcaId = id;

            return await marcaRepository.UpdateMarca(marca) ?
                Ok(new RespuestaBase { Mensaje = $"El registro de marca con el nombre de {marca.Nombre} ha sido actualizado" }) :
                Conflict(new RespuestaBase { Mensaje = $"El registro de marca con el nombre de {marca.Nombre} no pudo ser actualizado" });
        }

        [HttpDelete("id")]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Eliminar marca por ID (solo vendedores)")]
        [ProducesResponseType(200, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> DeleteMarca(int id)
        {
            var marca = await marcaRepository.GetMarca(id);
            if (marca == null)
            {
                return NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
            }

            if (marca.Productos.Count > 0)
            {
                return Conflict(new RespuestaBase { Mensaje = $"El registro {marca.Nombre} contiene productos asignados y no puede ser eliminado" });
            }

            return await marcaRepository.DeleteMarca(marca) ?
                Ok(new RespuestaBase { Mensaje = $"El registro de marca con el nombre de {marca.Nombre} ha sido eliminado" }) :
                Conflict(new RespuestaBase { Mensaje = $"El registro de marca con el nombre de {marca.Nombre} no pudo ser eliminado" });
        }
    }
}
