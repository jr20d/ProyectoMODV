using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoModV.DTO.Productos;
using ProyectoModV.DTO.Respuestas;
using ProyectoModV.Models;
using ProyectoModV.Repository.IRepository;
using ProyectoModV.SecurityConfig;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoModV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoRepository productoRepository;
        private readonly IMapper mapper;
        private readonly IJwtGenerator jwtGenerator;
        private readonly IVendedorRepository vendedorRepository;
        private readonly ITipoProductoRepository tipoProductoRepository;
        private readonly IMarcaRepository marcaRepository;

        public ProductosController(IProductoRepository productoRepository, IMapper mapper,
            IJwtGenerator jwtGenerator, IVendedorRepository vendedorRepository, ITipoProductoRepository tipoProductoRepository,
            IMarcaRepository marcaRepository)
        {
            this.productoRepository = productoRepository;
            this.mapper = mapper;
            this.jwtGenerator = jwtGenerator;
            this.vendedorRepository = vendedorRepository;
            this.tipoProductoRepository = tipoProductoRepository;
            this.marcaRepository = marcaRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation("Obtener listado de productos con paginación y filtros de búsqueda")]
        [ProducesResponseType(200, Type = typeof(RespuestaListadoPagedDto<ProductoDto>))]
        public async Task<IActionResult> GetProductos([FromQuery(Name = "pagina")] int pagina = 1, [FromQuery(Name = "busqueda")] string? busqueda = "",
            [FromQuery(Name = "marca")] string? marca = "", [FromQuery(Name = "tipoProducto")] string? tipoProducto = "")
        {
            Task<List<Producto>> taskProductos = Task
                .Run(() => productoRepository.GetProductos(busqueda == null ? "" : busqueda, pagina, marca == null ? "" : marca, tipoProducto == null ? "" : tipoProducto));
            Task<int> taskCantidadRegistros = Task
                .Run(() => productoRepository.GetTotalProductos(busqueda == null ? "" : busqueda, marca == null ? "" : marca, tipoProducto == null ? "" : tipoProducto));

            await Task.WhenAll(taskProductos, taskCantidadRegistros);
            var productos = taskProductos.Result;
            var totalRegistros = taskCantidadRegistros.Result;

            return Ok(new RespuestaListadoPagedDto<ProductoDto>
            {
                Mensaje = "Resultados",
                Cantidad = productos.Count,
                Registros = mapper.Map<List<ProductoDto>>(productos),
                TotalRegistros = totalRegistros
            });
        }

        [HttpGet("my-products")]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Obtener listado de productos pertenecientes a un vendedor (solo vendedores)")]
        [ProducesResponseType(200, Type = typeof(RespuestaListadoPagedDto<ProductoDto>))]
        public async Task<IActionResult> GetMyProductos([FromQuery(Name = "pagina")] int pagina = 1, [FromQuery(Name = "busqueda")] string? busqueda = "",
            [FromQuery(Name = "marca")] string? marca = "", [FromQuery(Name = "tipoProducto")] string? tipoProducto = "")
        {
            var idVendedor = await vendedorRepository.GetIdVendedor(jwtGenerator.GetIdUsuarioActual());

            Task<List<Producto>> taskProductos = Task
                .Run(() => productoRepository.GetProductos(busqueda == null ? "" : busqueda, pagina, marca == null ? "" : marca, tipoProducto == null ? "" : tipoProducto, idVendedor));
            Task<int> taskCantidadRegistros = Task
                .Run(() => productoRepository.GetTotalProductos(busqueda == null ? "" : busqueda, marca == null ? "" : marca, tipoProducto == null ? "" : tipoProducto, idVendedor));

            await Task.WhenAll(taskProductos, taskCantidadRegistros);
            var productos = taskProductos.Result;
            var totalRegistros = taskCantidadRegistros.Result;

            return Ok(new RespuestaListadoPagedDto<ProductoDto>
            {
                Mensaje = "Resultados",
                Cantidad = productos.Count,
                Registros = mapper.Map<List<ProductoDto>>(productos),
                TotalRegistros = totalRegistros
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation("Obtener producto por ID")]
        [ProducesResponseType(200, Type = typeof(ProductoDto))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        public async Task<IActionResult> GetProducto(int id)
        {
            var producto = await productoRepository.GetProducto(id);

            if (producto == null)
            {
                return NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
            }

            return Ok(mapper.Map<ProductoDto>(producto));
        }

        [HttpPost]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Agregar nuevo producto (solo vendedores)")]
        [ProducesResponseType(201, Type = typeof(ProductoBaseDto))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(400, Type = typeof(RespuestaBase))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateProducto([FromBody()] DataProductoDto productoDto)
        {
            var idUsuarioSesion = jwtGenerator.GetIdUsuarioActual();
            var idVendedor = await vendedorRepository.GetIdVendedor(idUsuarioSesion);
            if (!await tipoProductoRepository.ExistsTipoProducto(productoDto.TipoProductoId))
            {
                return Conflict(new RespuestaBase { Mensaje = "El tipo de producto especificado no existe" });
            }
            if (!await marcaRepository.ExistsMarca(productoDto.MarcaId))
            {
                return Conflict(new RespuestaBase { Mensaje = "La marca específicada no existe" });
            }
            if (await productoRepository.ExistsProducto(productoDto.Nombre, idVendedor))
            {
                return Conflict(new RespuestaBase { Mensaje = $"Ya existe un producto registrado con el nombre de {productoDto.Nombre}" });
            }

            var producto = mapper.Map<Producto>(productoDto);
            producto.VendedorId = idVendedor;
            return await productoRepository.CreateProducto(producto) ?
                Created($"/api/productos/{producto.ProductoId}", mapper.Map<ProductoDto>(producto)) :
                Conflict(new RespuestaBase { Mensaje = $"El producto {producto.Nombre} no pudo ser agregado" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Actualizar producto por ID (solo vendedores)")]
        [ProducesResponseType(200, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(400, Type = typeof(RespuestaBase))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateProducto([FromBody()] DataProductoDto productoDto, int id)
        {
            var idUsuarioSesion = jwtGenerator.GetIdUsuarioActual();
            var idVendedor = await vendedorRepository.GetIdVendedor(idUsuarioSesion);
            if (!await productoRepository.ExistsProducto(id, idVendedor))
            {
                return NotFound(new RespuestaBase { Mensaje = "El producto no existe" });
            }
            if (await productoRepository.ExistsProducto(id, productoDto.Nombre, idVendedor))
            {
                return Conflict(new RespuestaBase { Mensaje = $"Ya existe un producto con el nombre {productoDto.Nombre}" });
            }
            if (!await tipoProductoRepository.ExistsTipoProducto(productoDto.TipoProductoId))
            {
                return Conflict(new RespuestaBase { Mensaje = "El tipo de producto específicado no existe" });
            }
            if (!await marcaRepository.ExistsMarca(productoDto.MarcaId))
            {
                return Conflict(new RespuestaBase { Mensaje = "La marca específicada no existe" });
            }

            var producto = mapper.Map<Producto>(productoDto);
            producto.ProductoId = id;
            producto.VendedorId = idVendedor;
            return await productoRepository.UpdateProducto(producto) ?
                Ok(new RespuestaBase { Mensaje = "Los datos del producto han sido actualizados" }) :
                Conflict(new RespuestaBase { Mensaje = "Los datos del producto no fueron actualizados" });
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Actualizar/agregar cantidad de productos por ID (solo vendedores)")]
        [ProducesResponseType(200, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(400, Type = typeof(RespuestaBase))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateStock([FromBody()] ActualizarStockDto stockDto, int id)
        {
            var idUsuarioSesion = jwtGenerator.GetIdUsuarioActual();
            var idVendedor = await vendedorRepository.GetIdVendedor(idUsuarioSesion);
            if (!await productoRepository.ExistsProducto(id, idVendedor))
            {
                return NotFound(new RespuestaBase { Mensaje = "El producto no existe" });
            }

            var producto = await productoRepository.GetProducto(idVendedor);

            if (producto == null)
            {
                return NotFound(new RespuestaBase { Mensaje = "El producto no existe" });
            }

            var cantidadAnterior = producto.Cantidad;
            producto.Cantidad += stockDto.Cantidad;

            return await productoRepository.UpdateProducto(producto) ?
                Ok(new RespuestaBase { Mensaje = $"La cantidad del producto {producto.Nombre} se ha actualizado de {cantidadAnterior} a {producto.Cantidad}" }) :
                Conflict(new RespuestaBase { Mensaje = $"No se pudo actualizar la cantidad del producto {producto.Nombre}" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "vendedor")]
        [SwaggerOperation("Eliminar producto por ID (solo vendedores)")]
        [ProducesResponseType(200, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var idUsuarioSesion = jwtGenerator.GetIdUsuarioActual();
            var idVendedor = await vendedorRepository.GetIdVendedor(idUsuarioSesion);
            if (!await productoRepository.ExistsProducto(id, idVendedor))
            {
                return NotFound(new RespuestaBase { Mensaje = "El producto no existe" });
            }

            return await productoRepository.DeleteProducto(id) ?
                Ok(new RespuestaBase { Mensaje = "El producto ha sido eliminado" }) :
                Conflict(new RespuestaBase { Mensaje = "El producto no pudo ser eliminado" });
        }
    }
}