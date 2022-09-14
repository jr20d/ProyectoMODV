using FluentValidation;

namespace ProyectoModV.DTO.Productos
{
    public class ActualizarStockValidator : AbstractValidator<ActualizarStockDto>
    {
        public ActualizarStockValidator()
        {
            RuleFor(p => p.Cantidad).NotNull().GreaterThan(0);
        }
    }
    public class ActualizarStockDto
    {
        public int Cantidad { get; set; }
    }
}
