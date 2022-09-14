using Microsoft.EntityFrameworkCore;
using ProyectoModV.Data;
using ProyectoModV.Models;
using ProyectoModV.Repository.IRepository;

namespace ProyectoModV.Repository
{
    public class VendedorRepository : IVendedorRepository
    {
        private readonly TiendaDbContext context;

        public VendedorRepository(TiendaDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> DeleteVendedor(Vendedor vendedor)
        {
            context.Vendedores.Remove(vendedor);
            return await Save();
        }

        public async Task<Vendedor> GetVendedor(string id)
        {
            return await context.Vendedores.Include(v => v.Usuario).FirstOrDefaultAsync(v => v.UsuarioId == id);
        }

        public async Task<int> GetIdVendedor(string id)
        {
            return await context.Vendedores.AsQueryable().Where(v => v.UsuarioId == id).Select(v => v.VendedorId).SingleOrDefaultAsync();
        }

        public async Task<bool> CreateVendedor(Vendedor vendedor)
        {
            await context.Vendedores.AddAsync(vendedor);
            return await Save();
        }

        private async Task<bool> Save()
        {
            return await context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
