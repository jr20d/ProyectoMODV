using ProyectoModV.DTO.Marcas;
using ProyectoModV.DTO.TiposProductos;
using ProyectoModV.DTO.Vendedores;

namespace ProyectoModV.DTO.Productos
{
    public class ProductoDto : ProductoBaseDto
    {
        public int ProductoId { get; set; }
        public string Imagen { get; set; }
        public MarcaResultDto Marca { get; set; }
        public TipoProductoResultDto TipoProducto { get; set; }
        public VendedorDto Vendedor { get; set; }
    }
}
