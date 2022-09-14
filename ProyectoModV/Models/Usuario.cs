using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProyectoModV.Models
{
    public class Usuario : IdentityUser
    {
        [MaxLength(150)]
        public string Nombre { get; set; }
        [MaxLength(250)]
        public string? Imagen { get; set; }
        [MaxLength(250)]
        public string? PublicId { get; set; }
        
        public ICollection<Vendedor> Vendedores { get; set; }
        public ICollection<Venta> Ventas { get; set; }
    }
}
