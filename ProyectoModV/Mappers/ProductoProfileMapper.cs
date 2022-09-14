using AutoMapper;
using ProyectoModV.DTO.Productos;
using ProyectoModV.Models;

namespace ProyectoModV.Mappers
{
    public class ProductoProfileMapper : Profile
    {
        public ProductoProfileMapper()
        {
            //De Model a DTO
            CreateMap<Producto, ProductoDto>();

            //De DTO a Model
            CreateMap<DataProductoDto, Producto>().ReverseMap();
        }
    }
}
