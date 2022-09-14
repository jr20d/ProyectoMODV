using ProyectoModV.DTO.Productos;

namespace ProyectoModV.DTO.Ventas
{
    public class VentaDetalleDto : VentaDetalleBaseDto
    {
        public int VentaDetalleId { get; set; }
        public ProductoDto? Producto { get; set; }
    }
}
