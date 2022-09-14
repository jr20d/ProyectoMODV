using Microsoft.EntityFrameworkCore;
using ProyectoModV.Data;
using ProyectoModV.Models;
using ProyectoModV.Repository.IRepository;

namespace ProyectoModV.Repository
{
    public class TipoProductoRepository : ITipoProductoRepository
    {
        private readonly TiendaDbContext context;

        public TipoProductoRepository(TiendaDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> CreateTipoProducto(TipoProducto tipoProducto)
        {
            await context.TiposProducto.AddAsync(tipoProducto);
            return await Save();
        }

        public async Task<bool> DeleteTipoProducto(TipoProducto tipoProducto)
        {
            context.TiposProducto.Remove(tipoProducto);
            return await Save();
        }

        public async Task<bool> ExistsTipoProducto(int id)
        {
            return await context.TiposProducto.AnyAsync(tp => tp.TipoProductoId == id);
        }

        public async Task<bool> ExistsTipoProducto(string nombre)
        {
            return await context.TiposProducto.AnyAsync(tp => tp.Tipo.ToLower().Trim() == nombre.ToLower().Trim());
        }

        public async Task<bool> ExistsTipoProducto(int id, string nombre)
        {
            return await context.TiposProducto.AnyAsync(tp => tp.Tipo.ToLower().Trim() == nombre.ToLower().Trim() && tp.TipoProductoId != id);
        }

        public async Task<TipoProducto?> GetTipoProducto(int id)
        {
            return await context.TiposProducto.Include(tp => tp.Productos).FirstOrDefaultAsync(tp => tp.TipoProductoId == id);
        }

        public async Task<List<TipoProducto>> GetTiposProductos()
        {
            return await context.TiposProducto.Include(tp => tp.Productos).OrderBy(tp => tp.Tipo).ToListAsync();
        }

        public async Task<bool> UpdateTipoProducto(TipoProducto tipoProducto)
        {
            context.TiposProducto.Update(tipoProducto);
            return await Save();
        }

        private async Task<bool> Save()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
