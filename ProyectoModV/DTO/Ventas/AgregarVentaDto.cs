using FluentValidation;

namespace ProyectoModV.DTO.Ventas
{
    public class AgregarVentaValidator : AbstractValidator<AgregarVentaDto>
    {
        public AgregarVentaValidator()
        {
            RuleFor(v => v.Total).NotNull().GreaterThan(0);
            RuleFor(v => v.Numero).NotEmpty();
            RuleFor(v => v.Mes).GreaterThan(1);
            RuleFor(v => v.Year).GreaterThan(1);
            RuleFor(v => v.cvc).NotEmpty();
            RuleFor(v => v.Detalle).NotEmpty();
        }
    }
    public class AgregarVentaDto : VentaBaseDto
    {
        public string Numero { get; set; }
        public int Mes { get; set; }
        public int Year { get; set; }
        public string cvc { get; set; }
        public List<AgregarVentaDetalleDto> Detalle { get; set; }
    }
}
