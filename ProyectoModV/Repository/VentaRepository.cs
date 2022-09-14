using Microsoft.EntityFrameworkCore;
using ProyectoModV.Data;
using ProyectoModV.Models;
using ProyectoModV.Repository.IRepository;

namespace ProyectoModV.Repository
{
    public class VentaRepository : IVentaRepository
    {
        private readonly TiendaDbContext context;

        public VentaRepository(TiendaDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> CreateVenta(Venta venta)
        {
            await context.Ventas.AddAsync(venta);

            venta.Detalle.ToList().ForEach(p =>
            {
                var producto = context.Productos.Find(p.ProductoId);
                producto.Cantidad -= p.Cantidad;
                context.Productos.Update(producto);
            });
            return await Save();
        }

        public async Task<int> GetCantidadVentas(string usuarioId)
        {
            return await context.Ventas.AsQueryable()
                .Where(v => v.UsuarioId == usuarioId)
                .CountAsync();
        }

        public async Task<Venta?> GetVenta(int id)
        {
            try
            {
                return await context.Ventas
                .Include(v => v.Detalle)
                .ThenInclude(p => p.Producto)
                .ThenInclude(vp => vp.Vendedor)
                .ThenInclude(uv => uv.Usuario)
                .Include(v => v.Usuario)
                .Where(v => v.VentaId == id)
                .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }            
        }

        public async Task<List<Venta>> GetVentas(string usuarioId)
        {
            try
            {
                return await context.Ventas
                    .Include(v => v.Detalle)
                    .ThenInclude(p => p.Producto)
                    .ThenInclude(vp => vp.Vendedor)
                    .ThenInclude(uv => uv.Usuario)
                    .Include(u => u.Usuario)
                    .Where(v => v.UsuarioId == usuarioId)
                    .ToListAsync();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private async Task<bool> Save()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
