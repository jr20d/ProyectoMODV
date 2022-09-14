using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoModV.Models
{
    public class Vendedor
    {
        [Key]
        public int VendedorId { get; set; }
        
        [Column(name: "Id")]
        public string UsuarioId { get; set; }

        public Usuario Usuario { get; set; }

        public ICollection<Producto> Productos { get; set; }
    }
}
