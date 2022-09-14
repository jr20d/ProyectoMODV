using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoModV.Models
{
    public class Venta
    {
        [Key]
        public int VentaId { get; set; }
        [MaxLength(250)]
        public string Referencia { get; set; }
        [Column(TypeName = "decimal(14, 4)")]
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
        [Column("UsuarioId")]
        public string? UsuarioId { get; set; }

        public Usuario? Usuario { get; set; }
        public ICollection<VentaDetalle> Detalle { get; set; }
    }
}