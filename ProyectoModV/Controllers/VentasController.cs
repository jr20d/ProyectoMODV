﻿using AutoMapper;
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
    [ApiExplorerSettings(GroupName = "ApiVentas")]
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
        public async Task<IActionResult> CreateVenta([FromBody()] NuevaVentaDto ventaDto)
        {
            var idUsuarioSesion = jwtGenerator.GetIdUsuarioActual();
            if (ventaDto.Detalle.Count == 0)
            {
                return Conflict(new RespuestaBase { Mensaje = "Asegurese de agregar productos a la venta" });
            }

            List<RespuestaBase> respuestas = new();
            //Ordenar detalle con ID's de productos y cantidades
            List<DetalleNuevaVenta> detalleIds = new();
            //Listado de para realizar calculo
            //Objeto para almacenar valores en la base de datos
            List<VentaDetalle> ventaDetalle = new();

            Venta venta = new();

            ventaDto.Detalle.OrderBy(d => d.ProductoId);

            ventaDto.Detalle.ForEach(d =>
            {
                if (detalleIds.Any(data => data.ProductoId == d.ProductoId))
                {
                    detalleIds[detalleIds.FindIndex(data => data.ProductoId == d.ProductoId)].Cantidad += d.Cantidad;
                }
                else
                {
                    detalleIds.Add(d);
                }
            });

            detalleIds.ForEach(d =>
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
                else
                {
                    ventaDetalle.Add(new VentaDetalle
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        Precio = producto.Precio,
                        Descuento = producto.Descuento,
                        Importe = (producto.Precio * d.Cantidad) - (producto.Descuento * d.Cantidad)
                    });
                    venta.Total += (producto.Precio * d.Cantidad) - (producto.Descuento * d.Cantidad);
                }
            });

            if (respuestas.Count > 0)
            {
                return Conflict(respuestas);
            }

            venta.Referencia = Guid.NewGuid().ToString();
            venta.Fecha = DateTime.UtcNow.Date;
            venta.UsuarioId = idUsuarioSesion;
            venta.Detalle = ventaDetalle;

            var url = "http://localhost:8080/api/pagos";
            var peticion = (HttpWebRequest)WebRequest.Create(url);
            var obj = new
            {
                total = venta.Total,
                tarjeta = new
                {
                    numero = ventaDto.Numero,
                    mes = ventaDto.Mes,
                    year = ventaDto.Year,
                    cvc = ventaDto.Cvc
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
                            string respuesta = JsonConvert.DeserializeObject<dynamic>(objReader.ReadToEnd()).resultado ?? "";
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
