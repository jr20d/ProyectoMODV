using AutoMapper;
using ProyectoModV.DTO.TiposProductos;
using ProyectoModV.Models;

namespace ProyectoModV.Mappers
{
    public class TipoProductoProfileMapper : Profile
    {
        public TipoProductoProfileMapper()
        {
            //De Model a DTO
            CreateMap<TipoProducto, TipoProductoDto>()
                .ForMember(tpDestino => tpDestino.CantidadProductos, tpOrigen => tpOrigen.MapFrom(tp => tp.Productos.Count));
            CreateMap<TipoProducto, TipoProductoResultDto>();

            //De Dto a Model
            CreateMap<TipoProductoBaseDto, TipoProducto>();
        }
    }
}