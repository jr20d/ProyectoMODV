using FluentValidation;

namespace ProyectoModV.DTO.TiposProductos
{
    public class TipoProductoValidator : AbstractValidator<TipoProductoBaseDto>
    {
        public TipoProductoValidator()
        {
            RuleFor(tp => tp.Tipo).NotEmpty().MinimumLength(2).MaximumLength(75);
        }
    }
    public class TipoProductoBaseDto
    {
        public string Tipo { get; set; }
    }
}
