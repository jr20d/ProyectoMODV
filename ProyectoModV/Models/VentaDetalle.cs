using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoModV.Models
{
    public class VentaDetalle
    {
        [Key]
        public int VentaDetalleId { get; set; }
        [Column(TypeName = "decimal(14, 4)")]
        public decimal Precio { get; set; }
        [Column(TypeName = "decimal(14, 4)")]
        public decimal Descuento { get; set; }
        public int Cantidad { get; set; }
        [Column(TypeName = "decimal(14, 4)")]
        public decimal Importe { get; set; }
        [Column("VentaId")]
        public int VentaId { get; set; }
        [Column("ProductoId")]
        public int? ProductoId { get; set; }

        public Venta Venta { get; set; }
        public Producto? Producto { get; set; }
    }
}
