using ProyectoModV.Models;

namespace ProyectoModV.Repository.IRepository
{
    public interface IVentaRepository
    {
        Task<List<Venta>> GetVentas(string usaurioId);
        Task<Venta?> GetVenta(int id);
        Task<int> GetCantidadVentas(string usuarioId);
        Task<bool> CreateVenta(Venta venta);
    }
}