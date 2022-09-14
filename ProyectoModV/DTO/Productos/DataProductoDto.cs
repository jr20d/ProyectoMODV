using FluentValidation;

namespace ProyectoModV.DTO.Productos
{
    public class DataProductoValidator : AbstractValidator<DataProductoDto>
    {
        public DataProductoValidator()
        {
            RuleFor(p => p.Nombre).NotEmpty().MinimumLength(2).MaximumLength(250);
            RuleFor(p => p.Modelo).NotEmpty().MinimumLength(2).MaximumLength(150);
            RuleFor(p => p.Precio).NotNull().GreaterThan(0);
            RuleFor(p => p.Descuento).NotNull().GreaterThanOrEqualTo(0);
            RuleFor(p => p.Cantidad).NotNull().GreaterThanOrEqualTo(0);
            RuleFor(p => p.MarcaId).NotNull().GreaterThan(0);
            RuleFor(p => p.TipoProductoId).NotNull().GreaterThan(0);
        }
    }
    public class DataProductoDto : ProductoBaseDto
    {
        public int MarcaId { get; set; }
        public int TipoProductoId { get; set; }
    }
}
