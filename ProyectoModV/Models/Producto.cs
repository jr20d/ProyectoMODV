using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoModV.Models
{
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }
        [MaxLength(250)]
        public string Nombre { get; set; }
        [MaxLength(150)]
        public string Modelo { get; set; }
        [Column(TypeName = "decimal(14, 4)")]
        public decimal Precio { get; set; }
        [Column(TypeName = "decimal(14, 4)")]
        public decimal Descuento { get; set; }
        public int Cantidad { get; set; }
        [MaxLength(250)]
        public string? Imagen { get; set; }
        [MaxLength(200)]
        public string? PublicId { get; set; }

        [Column("MarcaId")]
        public int MarcaId { get; set; }

        [Column("TipoProductoId")]
        public int TipoProductoId { get; set; }

        [Column(name: "VendedorId")]
        public int VendedorId { get; set; }

        public Marca Marca { get; set; }
        public TipoProducto TipoProducto { get; set; }
        public Vendedor Vendedor { get; set; }
        public ICollection<VentaDetalle> VentaDetalles { get; set; }
    }
}
