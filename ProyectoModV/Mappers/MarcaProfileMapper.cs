using AutoMapper;
using ProyectoModV.DTO.Marcas;
using ProyectoModV.Models;

namespace ProyectoModV.Mappers
{
    public class MarcaProfileMapper : Profile
    {
        public MarcaProfileMapper()
        {
            //De Model a DTO
            CreateMap<Marca, MarcaDto>()
                .ForMember(mDest => mDest.CantidadProductos, mOrigen => mOrigen.MapFrom(m => m.Productos.Count));
            CreateMap<Marca, MarcaResultDto>();

            //De DTO a Model
            CreateMap<MarcaBaseDto, Marca>();
        }
    }
}
