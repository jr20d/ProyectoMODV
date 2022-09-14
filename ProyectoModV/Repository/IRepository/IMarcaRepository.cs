using ProyectoModV.Models;

namespace ProyectoModV.Repository.IRepository
{
    public interface IMarcaRepository
    {
        Task<List<Marca>> GetMarcas();
        Task<Marca?> GetMarca(int id);
        Task<bool> ExistsMarca(int id);
        Task<bool> ExistsMarca(string nombre);
        Task<bool> ExistsMarca(int id, string nombre);
        Task<bool> CreateMarca(Marca marca);
        Task<bool> UpdateMarca(Marca marca);
        Task<bool> DeleteMarca(Marca marca);
    }
}
