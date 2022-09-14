using ProyectoModV.DTO.Productos;
using ProyectoModV.Models;

namespace ProyectoModV.Repository.IRepository
{
    public interface IProductoRepository
    {
        Task<List<Producto>> GetProductos(string busqueda, int pagina, string marca, string tipoProducto);
        Task<List<Producto>> GetProductos(string busqueda, int pagina, string marca, string tipoProducto, int vendedorId);
        Task<int> GetTotalProductos(string busqueda, string marca, string tipoProducto);
        Task<int> GetTotalProductos(string busqueda, string marca, string tipoProducto, int vendedorId);
        Task<Producto?> GetProducto(int id);
        Task<ProductoIdNombre?> GetIdAndNombreProducto(int id);
        Task<int> GetCantidadProducto(int id);
        Task<bool> ExistsProducto(int productoId, int vendedorId);
        Task<bool> ExistsProducto(string nombre, int vendedorId);
        Task<bool> ExistsProducto(int productoId, string nombre, int vendedorId);
        Task<bool> CreateProducto(Producto producto);
        Task<bool> UpdateProducto(Producto producto);
        Task<bool> DeleteProducto(int productoId);
    }
}
