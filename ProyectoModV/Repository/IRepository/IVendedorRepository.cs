using ProyectoModV.Models;

namespace ProyectoModV.Repository.IRepository
{
    public interface IVendedorRepository
    {
        Task<Vendedor> GetVendedor(string id);
        Task<int> GetIdVendedor(string id);
        Task<bool> CreateVendedor(Vendedor vendedor);
        Task<bool> DeleteVendedor(Vendedor vendedor);
    }
}
