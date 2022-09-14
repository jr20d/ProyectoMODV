using System.ComponentModel.DataAnnotations;

namespace ProyectoModV.Models
{
    public class TipoProducto
    {
        [Key]
        public int TipoProductoId { get; set; }
        [MaxLength(75)]
        public string Tipo { get; set; }

        public ICollection<Producto> Productos { get; set; }
    }
}
