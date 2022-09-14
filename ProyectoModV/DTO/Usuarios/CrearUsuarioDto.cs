using FluentValidation;

namespace ProyectoModV.DTO.Usuarios
{
    public class CrearUsuarioValidator : AbstractValidator<CrearUsuarioDto>
    {
        public CrearUsuarioValidator()
        {
            RuleFor(u => u.Nombre).NotEmpty().MinimumLength(3).MaximumLength(150);
            RuleFor(u => u.Telefono).NotEmpty().Matches(@"^[0-9]{4}\-\d{4}$");
            RuleFor(u => u.Correo).NotEmpty().EmailAddress();
            RuleFor(u => u.Password).NotEmpty().MinimumLength(5).Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-.]).{5,}$");
        }
    }
    public class CrearUsuarioDto : BaseUsuarioDto
    {
        public string Password { get; set; }
    }
}