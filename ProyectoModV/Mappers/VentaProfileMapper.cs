using AutoMapper;
using ProyectoModV.DTO.Productos;
using ProyectoModV.DTO.Ventas;
using ProyectoModV.Models;

namespace ProyectoModV.Mappers
{
    public class VentaProfileMapper : Profile
    {
        public VentaProfileMapper()
        {
            //De Model a DTO
            CreateMap<Venta, VentaDto>();
            CreateMap<VentaDetalle, VentaDetalleDto>();

            //De DTO a Model
            CreateMap<AgregarVentaDto, Venta>();
            CreateMap<AgregarVentaDetalleDto, VentaDetalle>();
        }
    }
}
