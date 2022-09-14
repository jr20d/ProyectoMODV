using FluentValidation;

namespace ProyectoModV.DTO.Usuarios
{
    public class ActualizarUsuarioValidator : AbstractValidator<ActualizarUsuarioDto>
    {
        public ActualizarUsuarioValidator()
        {
            RuleFor(u => u.Nombre).NotEmpty().MinimumLength(3).MaximumLength(150);
            RuleFor(u => u.Telefono).NotEmpty().Matches(@"^[0-9]{4}\-\d{4}$");
            RuleFor(u => u.Correo).NotEmpty().EmailAddress();
        }
    }
    public class ActualizarUsuarioDto : BaseUsuarioDto
    {
        
    }
}
