using Microsoft.EntityFrameworkCore;
using ProyectoModV.Data;
using ProyectoModV.Models;
using ProyectoModV.Repository.IRepository;
using System.Linq;

namespace ProyectoModV.Repository
{
    public class MarcaRepository : IMarcaRepository
    {
        private readonly TiendaDbContext dbContext;

        public MarcaRepository(TiendaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateMarca(Marca marca)
        {
            await dbContext.Marcas.AddAsync(marca);
            return await Save();
        }

        public async Task<bool> DeleteMarca(Marca marca)
        {
            dbContext.Marcas.Remove(marca);
            return await Save();
        }

        public async Task<bool> ExistsMarca(int id)
        {
            return await dbContext.Marcas.AnyAsync(m => m.MarcaId == id);
        }

        public async Task<bool> ExistsMarca(string nombre)
        {
            return await dbContext.Marcas.AnyAsync(m => m.Nombre.Trim().ToLower() == nombre.Trim().ToLower());
        }

        public async Task<bool> ExistsMarca(int id, string nombre)
        {
            return await dbContext.Marcas.AnyAsync(m => m.Nombre.Trim().ToLower() == nombre.Trim().ToLower() && m.MarcaId != id);
        }

        public async Task<Marca?> GetMarca(int id)
        {
            return await dbContext.Marcas.Include(m => m.Productos).FirstOrDefaultAsync(m => m.MarcaId == id);
        }

        public async Task<List<Marca>> GetMarcas()
        {
            return await dbContext.Marcas.Include(m => m.Productos).OrderBy(m => m.Nombre).ToListAsync();
        }

        public async Task<bool> UpdateMarca(Marca marca)
        {
            dbContext.Marcas.Update(marca);
            return await Save();
        }

        private async Task<bool> Save()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}
