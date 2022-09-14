using FluentValidation;

namespace ProyectoModV.DTO.Usuarios
{
    public class ActualizarPasswordvalidator : AbstractValidator<ActualizarPasswordDto>
    {
        public ActualizarPasswordvalidator()
        {
            RuleFor(u => u.Password).NotEmpty().MinimumLength(5).Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-.]).{5,}$");
            RuleFor(u => u.Confirmacion).NotEmpty().MinimumLength(5).Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-.]).{5,}$");
            RuleFor(u => u.PasswordAnterior).NotEmpty().MinimumLength(5).Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-.]).{5,}$");
        }
    }
    public class ActualizarPasswordDto
    {
        public string Password { get; set; }
        public string Confirmacion { get; set; }
        public string PasswordAnterior { get; set; }
    }
}