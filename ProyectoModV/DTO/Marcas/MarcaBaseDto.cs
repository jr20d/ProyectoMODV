using FluentValidation;

namespace ProyectoModV.DTO.Marcas
{
    public class MarcaBaseValidator : AbstractValidator<MarcaBaseDto>
    {
        public MarcaBaseValidator()
        {
            RuleFor(m => m.Nombre).NotEmpty().MinimumLength(2).MaximumLength(50);
        }
    }
    public class MarcaBaseDto
    {
        public string Nombre { get; set; }
    }
}
