using ProyectoModV.Models;

namespace ProyectoModV.Repository.IRepository
{
    public interface ITipoProductoRepository
    {
        Task<List<TipoProducto>> GetTiposProductos();
        Task<TipoProducto?> GetTipoProducto(int id);
        Task<bool> ExistsTipoProducto(int id);
        Task<bool> ExistsTipoProducto(string nombre);
        Task<bool> ExistsTipoProducto(int id, string nombre);
        Task<bool> CreateTipoProducto(TipoProducto tipoProducto);
        Task<bool> UpdateTipoProducto(TipoProducto tipoProducto);
        Task<bool> DeleteTipoProducto(TipoProducto tipoProducto);
    }
}
