using AutoMapper;
using ProyectoModV.DTO.Usuarios;
using ProyectoModV.DTO.Vendedores;
using ProyectoModV.Models;

namespace ProyectoModV.Mappers
{
    public class VendedorProfileMapper : Profile
    {
        public VendedorProfileMapper()
        {
            //De Model a DTO
            CreateMap<Vendedor, VendedorDto>().ReverseMap();
        }
    }
}
