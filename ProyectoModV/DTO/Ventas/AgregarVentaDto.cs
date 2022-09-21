using FluentValidation;

namespace ProyectoModV.DTO.Ventas
{
    /*public class AgregarVentaValidator : AbstractValidator<AgregarVentaDto>
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
    }*/
    public class AgregarVentaDto : VentaBaseDto
    {
        public List<AgregarVentaDetalleDto> Detalle { get; set; }
    }

    public class NuevaVentaValidator : AbstractValidator<NuevaVentaDto>
    {
        public NuevaVentaValidator()
        {
            RuleFor(v => v.Numero).NotEmpty();
            RuleFor(v => v.Mes).GreaterThan(1);
            RuleFor(v => v.Year).GreaterThan(1);
            RuleFor(v => v.Cvc).NotEmpty();
            RuleFor(v => v.Detalle).NotEmpty();
        }
    }
    public class NuevaVentaDto
    {
        public string Numero { get; set; }
        public int Mes { get; set; }
        public int Year { get; set; }
        public string Cvc { get; set; }
        public List<DetalleNuevaVenta> Detalle { get; set; }
    }
    public class DetalleNuevaVenta
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
