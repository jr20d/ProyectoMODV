namespace ProyectoModV.DTO.Ventas
{
    public class VentaDetalleBaseDto
    {
        public decimal Precio { get; set; }
        public decimal Descuento { get; set; }
        public int Cantidad { get; set; }
        public decimal Importe { get; set; }
    }
}
