using AutoMapper;
using ProyectoModV.DTO.Usuarios;
using ProyectoModV.Models;
using ProyectoModV.SecurityConfig;

namespace ProyectoModV.Mappers
{
    public class UsuarioProfileMapper : Profile
    {
        public UsuarioProfileMapper()
        {
            //De DTO a Model
            CreateMap<CrearUsuarioDto, Usuario>()
                .ForMember(uDest => uDest.Nombre, uOrigen => uOrigen.MapFrom(u => u.Nombre))
                .ForMember(uDest => uDest.Email, uOrigen => uOrigen.MapFrom(u => u.Correo))
                .ForMember(uDest => uDest.UserName, uOrigen => uOrigen.MapFrom(u => u.Correo))
                .ForMember(uDest => uDest.PhoneNumber, uOrigen => uOrigen.MapFrom(u => u.Telefono));
            CreateMap<ActualizarUsuarioDto, Usuario>()
                .ForMember(uDest => uDest.Nombre, uOrigen => uOrigen.MapFrom(u => u.Nombre))
                .ForMember(uDest => uDest.Email, uOrigen => uOrigen.MapFrom(u => u.Correo))
                .ForMember(uDest => uDest.UserName, uOrigen => uOrigen.MapFrom(u => u.Correo))
                .ForMember(uDest => uDest.PhoneNumber, uOrigen => uOrigen.MapFrom(u => u.Telefono));

            //De Model a DTO            
            CreateMap<Usuario, LoggedUsuarioDto>()
                .ForMember(uDest => uDest.Nombre, uOrigen => uOrigen.MapFrom(u => u.Nombre))
                .ForMember(uDest => uDest.Correo, uOrigen => uOrigen.MapFrom(u => u.Email))
                .ForMember(uDest => uDest.Telefono, uOrigen => uOrigen.MapFrom(u => u.PhoneNumber));

            CreateMap<Usuario, UsuarioDto>()
                .ForMember(uDest => uDest.UsuarioId, uOrigen => uOrigen.MapFrom(u => u.Id))
                .ForMember(uDest => uDest.Nombre, uOrigen => uOrigen.MapFrom(u => u.Nombre))
                .ForMember(uDest => uDest.Correo, uOrigen => uOrigen.MapFrom(u => u.Email))
                .ForMember(uDest => uDest.Telefono, uOrigen => uOrigen.MapFrom(u => u.PhoneNumber));

            CreateMap<Usuario, UsuarioDetalleDto>()
                .ForMember(uDest => uDest.UsuarioId, uOrigen => uOrigen.MapFrom(u => u.Id))
                .ForMember(uDest => uDest.Nombre, uOrigen => uOrigen.MapFrom(u => u.Nombre))
                .ForMember(uDest => uDest.Correo, uOrigen => uOrigen.MapFrom(u => u.Email))
                .ForMember(uDest => uDest.Telefono, uOrigen => uOrigen.MapFrom(u => u.PhoneNumber));

            //De datos del JWT a Usuario Model
            CreateMap<DataUsuarioToken, Usuario>()
                .ForMember(uDest => uDest.Id, uOrigen => uOrigen.MapFrom(u => u.UsuarioId))
                .ForMember(uDest => uDest.Email, uOrigen => uOrigen.MapFrom(u => u.Email));
        }
    }
}
