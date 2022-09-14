using ProyectoModV.DTO.Usuarios;

namespace ProyectoModV.DTO.Ventas
{
    public class VentaDto : VentaBaseDto
    {
        public int VentaId { get; set; }
        public string Referencia { get; set; }
        public DateTime Fecha { get; set; }
        public UsuarioDto? Usuario { get; set; }
        public List<VentaDetalleDto> Detalle { get; set; }
    }
}
