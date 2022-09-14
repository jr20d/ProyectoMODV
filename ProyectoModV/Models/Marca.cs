using System.ComponentModel.DataAnnotations;

namespace ProyectoModV.Models
{
    public class Marca
    {
        public int MarcaId { get; set; }
        [MaxLength(50)]
        public string Nombre { get; set; }

        public ICollection<Producto> Productos { get; set; }
    }
}
