using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoModV.DTO.Respuestas;
using ProyectoModV.DTO.Ventas;
using ProyectoModV.Models;
using ProyectoModV.Repository.IRepository;
using ProyectoModV.SecurityConfig;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace ProyectoModV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly IVentaRepository ventaRepository;
        private readonly IProductoRepository productoRepository;
        private readonly IMapper mapper;
        private readonly IJwtGenerator jwtGenerator;

        public VentasController(IVentaRepository ventaRepository, IMapper mapper, IJwtGenerator jwtGenerator, IProductoRepository productoRepository)
        {
            this.ventaRepository = ventaRepository;
            this.mapper = mapper;
            this.jwtGenerator = jwtGenerator;
            this.productoRepository = productoRepository;
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation("Obtener listado de ventas")]
        [ProducesResponseType(200, Type = typeof(RespuestaListado<VentaDto>))]
        public async Task<IActionResult> GetVentas()
        {
            var usuarioId = jwtGenerator.GetIdUsuarioActual();
            try
            {
                var ventas = await ventaRepository.GetVentas(usuarioId);

                return Ok(new RespuestaListado<VentaDto>
                {
                    Mensaje = "Resultado",
                    Cantidad = ventas.Count,
                    Registros = mapper.Map<List<VentaDto>>(ventas)
                });
            }
            catch
            {
                return Ok(new RespuestaListado<VentaDto>
                {
                    Mensaje = "Resultado",
                    Cantidad = 0,
                    Registros = new List<VentaDto>()
                });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation("Obtener venta por ID")]
        [ProducesResponseType(200, Type = typeof(VentaDto))]
        [ProducesResponseType(404, Type = typeof(RespuestaBase))]
        public async Task<IActionResult> GetVenta(int id)
        {
            try
            {
                var venta = await ventaRepository.GetVenta(id);

                if (venta == null)
                {
                    return NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
                }

                return Ok(mapper.Map<VentaDto>(venta));
            }
            catch
            {
                return NotFound(new RespuestaBase { Mensaje = "El registro no existe" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "cliente")]
        [SwaggerOperation("Realizar una venta (solo clientes)")]
        [ProducesResponseType(201, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(RespuestaBase))]
        [ProducesResponseType(409, Type = typeof(List<RespuestaBase>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateVenta([FromBody()] AgregarVentaDto ventaDto)
        {
            var idUsuarioSesion = jwtGenerator.GetIdUsuarioActual();
            if (ventaDto.Detalle.Count == 0)
            {
                return Conflict(new RespuestaBase { Mensaje = "Asegurese de agregar productos a la venta" });
            }

            List<RespuestaBase> respuestas = new();
            List<AgregarVentaDetalleDto> detalle = new();

            ventaDto.Detalle.OrderBy(d => d.ProductoId);

            ventaDto.Detalle.ForEach(d =>
            {
                if (detalle.Any(data => data.ProductoId == d.ProductoId))
                {
                    detalle[detalle.FindIndex(data => data.ProductoId == d.ProductoId)].Cantidad += d.Cantidad;
                    detalle[detalle.FindIndex(data => data.ProductoId == d.ProductoId)].Descuento += d.Descuento;
                    detalle[detalle.FindIndex(data => data.ProductoId == d.ProductoId)].Importe += d.Importe;
                }
                else
                {
                    detalle.Add(d);
                }
            });

            detalle.ForEach(d =>
            {
                var producto = productoRepository.GetIdAndNombreProducto(d.ProductoId).Result;
                if (producto == null)
                {
                    respuestas.Add(new RespuestaBase { Mensaje = $"El producto con ID {d.ProductoId} no existe" });
                }
                else if (producto.Cantidad - d.Cantidad < 0)
                {
                    respuestas.Add(new RespuestaBase { Mensaje = $"La cantidad indicada del producto {producto.Nombre} supera la disponibilidad del mismo" });
                }
            });

            if (respuestas.Count > 0)
            {
                return Conflict(respuestas);
            }

            var venta = mapper.Map<Venta>(ventaDto);
            venta.Referencia = Guid.NewGuid().ToString();
            venta.Fecha = DateTime.UtcNow.Date;
            venta.UsuarioId = idUsuarioSesion;

            var url = "http://localhost:8080/api/pagos";
            var peticion = (HttpWebRequest)WebRequest.Create(url);
            var obj = new
            {
                total = ventaDto.Total,
                tarjeta = new
                {
                    numero = ventaDto.Numero,
                    mes = ventaDto.Mes,
                    year = ventaDto.Year,
                    cvc = ventaDto.cvc
                }
            };

            string json = JsonConvert.SerializeObject(obj);
            peticion.Method = HttpMethod.Post.Method;
            peticion.ContentType = "application/json";
            peticion.Accept = "application/json";

            using (var streamWriter = new StreamWriter(peticion.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                using (WebResponse response = peticion.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null)
                        {
                            return Conflict(new RespuestaBase { Mensaje = "La venta no pudo realizarse" });
                        }
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string respuesta = JsonConvert.DeserializeObject<dynamic>(objReader.ReadToEnd()).Resultado;
                            if (respuesta.ToString() == "El pago ha sido realizado")
                            {
                                return await ventaRepository.CreateVenta(venta) ?
                                Ok(new RespuestaBase { Mensaje = $"Va venta ha sido efectuada con el código de referencia {venta.Referencia}" }) :
                                Conflict(new RespuestaBase { Mensaje = "La venta no pudo realizarse" });
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                return Conflict(new RespuestaBase { Mensaje = "La venta no pudo realizarse" });
            }
            return Conflict(new RespuestaBase { Mensaje = "La venta no pudo realizarse" });
        }
    }
}
